using MathDrawerGame.AI.Animations;
using MathDrawerGame.AI.PathEquations;
using MathDrawerGame.Geom;
using MathDrawerGame.LevelEditor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame.AI
{
    class SimpleAITester : PointDebugger
    {
        double apg = 30; // meters per second squared (4.47=performance car, 12.2=fastest exotic car)
        public double ay = 9.8; // gravity, meters per second squared
        public static double PREC = 100;
        IAnimation2D animation = null;
        double timein = 0;
        ButtonState prevbutton = ButtonState.Released;
        internal void Update(HumanPlayer player, MainTerrain mainTerrain, Vector2D mouseRelativeCoords, double timediff)
        {
            base.Update(player, mainTerrain, mouseRelativeCoords);
            timein += timediff;
            // plan of attack:
            // create a custom class that indiciates an animatable path taken and the amount of time it took, just like the one we had from that other program
            // umm, copy that code but run it for each node across every move?
            // lets start off with like 3 moves
            // lets also just do the straighteq
            if (SelectedPoint != null && prevbutton == ButtonState.Pressed && Mouse.GetState().RightButton == ButtonState.Released)
            {
                SavableTerrain2D mainTerrainClone = mainTerrain.To2D();
                int i1 = mainTerrain.points.IndexOf(player.attached.p1);
                int i2 = mainTerrain.points.IndexOf(player.attached.p2);
                int l1 = mainTerrain.lines.IndexOf(player.attached);
                DiscreteFunction[] dfunctions = InitializeDFunctions(player, mainTerrainClone, i1, i2, l1);
                for (int it = 0; it < 6; it++) // just trying 3 moves currently
                {
                    DiscreteFunction[] newdfunctions = new DiscreteFunction[mainTerrainClone.points.Count]; // I think these functions were going to include negative values
                    for (int i = 0; i < mainTerrainClone.points.Count; i++) newdfunctions[i] = dfunctions[i]; // MISTAKE: didn't make a copy, meaning I'd only do paths that took 3 moves
                    // going to brute force everything for now, to insure things are working
                    // as I recall, I basically did this already
                    // for every point, get that points functions and just map them from every bloody thing 
                    for (int i = 0; i < dfunctions.Length; i++)
                    {
                        Vector2D thepoint = mainTerrainClone.points[i];
                        if (dfunctions[i] != null)
                        {
                            // get the connecting points
                            // TODO: we're going to decide for now that negative/positive speed is relative to the x component on the screen or whatever
                            var connecting = mainTerrainClone.Attached(thepoint);
                            Line2D leftline = connecting[0];
                            Line2D rightline = connecting[1];
                            // MISTAKE: because we're cheating, we need to decide if vertical is a leftline or rightline (i've chosen left)
                            if ((leftline.p1 == thepoint) ? (leftline.DX > 0) : (leftline.DX <= 0)) // swap
                            {
                                var temp = leftline;
                                leftline = rightline;
                                rightline = temp;
                            }
                            int lefti = mainTerrainClone.points.IndexOf(leftline.OtherP(thepoint));
                            int righti = mainTerrainClone.points.IndexOf(rightline.OtherP(thepoint));
                            double lefta1 = apg + leftline.AsVector2D().GravityMultiplier() * ay;
                            double lefta2 = -apg + leftline.AsVector2D().GravityMultiplier() * ay;
                            double righta1 = apg + rightline.AsVector2D().GravityMultiplier() * ay;
                            double righta2 = -apg + rightline.AsVector2D().GravityMultiplier() * ay;
                            foreach (var time in dfunctions[i].times.ToList()) // suddenly this is complaining about modification with the addition of returnfaster/returnslower?? using tolist to quiet it
                            {
                                // TODO: I believe a remaining bug just has to do with when a slope flips over
                                DoAThing(time, leftline, thepoint, lefti, -1, newdfunctions, new StraightEq(lefta1, lefta2, leftline.Length));
                                DoAThing(time, rightline, thepoint, righti, 1, newdfunctions, new StraightEq(righta1, righta2, rightline.Length));
                                DoAThing(time, leftline, thepoint, i, -1, newdfunctions, new ReturnFasterEq(lefta1, lefta2, leftline.Length));
                                DoAThing(time, rightline, thepoint, i, 1, newdfunctions, new ReturnFasterEq(righta1, righta2, rightline.Length));
                                DoAThing(time, leftline, thepoint, i, -1, newdfunctions, new ReturnSlowerEq(lefta1, lefta2, leftline.Length));
                                DoAThing(time, rightline, thepoint, i, 1, newdfunctions, new ReturnSlowerEq(righta1, righta2, rightline.Length));
                            }
                        }
                    }
                    dfunctions = newdfunctions;
                }
                int itarget = mainTerrain.points.IndexOf(SelectedPoint);
                if (dfunctions[itarget] != null && dfunctions[itarget].Get(0) != null)
                {
                    animation = dfunctions[itarget].Get(0);
                    timein = 0;
                }
            }
            prevbutton = Mouse.GetState().RightButton;
        }

        private void DoAThing(KeyValuePair<int, IAnimation2D> time, Line2D theline, Vector2D thepoint, int thei, int sign, DiscreteFunction[] newdfunctions, ISpeedTimeFunction path)
        {
            if (time.Key * sign >= 0)
            {
                Bounds vbounds = path.EBound(sign * time.Key / PREC);
                if (vbounds == null) return;
                int low = (int)Math.Ceiling(vbounds.low * PREC);
                int high = (int)(vbounds.high * PREC);
                Vector2D vector = theline.AsVector2DWithEnd(thepoint).Normalize(1) * -1;
                for (int j = low; j <= high; j++)
                {
                    IAnimation pathanim = path.Animate(sign * time.Key / PREC, j / PREC);
                    if (pathanim == null) continue; // this has happened, I think
                    IAnimation2D animation = new AnimationFrom1D(thepoint, vector, pathanim);
                    animation = time.Value + animation;
                    if (newdfunctions[thei] == null) newdfunctions[thei] = new DiscreteFunction();
                    if (!newdfunctions[thei].times.ContainsKey(sign * j) || animation.Time() < newdfunctions[thei].times[sign * j].Time())
                    {
                        newdfunctions[thei].Put(sign * j, animation);
                    }
                }
            }
        }

        private DiscreteFunction[] InitializeDFunctions(HumanPlayer player, SavableTerrain2D mainTerrain, int i1, int i2, int l1)
        {
            Vector2D startpoint = player.attached.At(player.p);
            mainTerrain.points.Add(startpoint);
            mainTerrain.lines.RemoveAt(l1);
            mainTerrain.lines.Add(new Line2D(mainTerrain.points[i1], startpoint));
            mainTerrain.lines.Add(new Line2D(startpoint, mainTerrain.points[i2]));
            DiscreteFunction[] dfunctions = new DiscreteFunction[mainTerrain.points.Count];
            DiscreteFunction startfunc = new DiscreteFunction();
            dfunctions[mainTerrain.points.Count - 1] = startfunc;
            // TODO: figure out gravity and accelerations and stuff
            // TODO: do the othe returnfaster/returnslower
            double v = player.vp * player.attached.Length;
            bool movingleft = (player.attached.DX < 0);
            startfunc.Put((int)Math.Round(v * PREC * (movingleft ? -1 : 1)), new EmptyAnimation2D());
            return dfunctions;
        }
        internal void Draw(BasicEffect basicEffect, GraphicsDevice GraphicsDevice)
        {
            if (animation != null)
            {
                Vector2D pos = animation.Pos(timein%animation.Time()); // loop the animation to help with debugging
                basicEffect.DrawPoint(GraphicsDevice, pos.x, pos.y, Color.Blue);
            }
        }
    }

    public class DiscreteFunction
    {
        public Dictionary<int, IAnimation2D> times = new Dictionary<int, IAnimation2D>();

        internal void Put(int i, IAnimation2D animation)
        {
            times[i] = animation;
        }

        internal IAnimation2D Get(int p)
        {
            if (!times.ContainsKey(p)) return null;
            return times[p];
        }
    }
}
