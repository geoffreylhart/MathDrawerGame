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
        public static double PREC = 10;
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
                DiscreteFunction dfunction = InitializeDFunction(player, mainTerrainClone, i1, i2, l1); // also alters the terrain
                DiscreteFunction[] dfunctions1 = new DiscreteFunction[mainTerrain.points.Count + 1];
                DiscreteFunction[] dfunctions2 = new DiscreteFunction[mainTerrain.points.Count + 1];
                dfunctions1[dfunctions1.Length - 1] = dfunction;
                if (player.vp == 0)
                {
                    dfunctions2[dfunctions1.Length - 1] = dfunction;
                }
                for (int it = 0; it < 3; it++) // just trying 3 moves currently
                {
                    DiscreteFunction[] newdfunctions1 = new DiscreteFunction[mainTerrainClone.points.Count]; // I think these functions were going to include negative values
                    DiscreteFunction[] newdfunctions2 = new DiscreteFunction[mainTerrainClone.points.Count];
                    for (int i = 0; i < mainTerrainClone.points.Count; i++) newdfunctions1[i] = dfunctions1[i]; // MISTAKE: didn't make a copy, meaning I'd only do paths that took 3 moves
                    for (int i = 0; i < mainTerrainClone.points.Count; i++) newdfunctions2[i] = dfunctions2[i]; 
                    // going to brute force everything for now, to insure things are working
                    // as I recall, I basically did this already
                    // for every point, get that points functions and just map them from every bloody thing 
                    for (int i = 0; i < mainTerrainClone.points.Count; i++)
                    {
                        // get the connecting points
                        Vector2D thepoint = mainTerrainClone.points[i];
                        var connecting = mainTerrainClone.Attached(thepoint);
                        DoAThing2(connecting[0], connecting[1], dfunctions1[i], newdfunctions2, newdfunctions1, newdfunctions2, i, mainTerrainClone);
                        DoAThing2(connecting[1], connecting[0], dfunctions2[i], newdfunctions1, newdfunctions1, newdfunctions2, i, mainTerrainClone);
                    }
                    dfunctions1 = newdfunctions1;
                    dfunctions2 = newdfunctions2;
                }
                int itarget = mainTerrain.points.IndexOf(SelectedPoint);
                if (dfunctions1[itarget] != null && dfunctions1[itarget].Get(0) != null)
                {
                    animation = dfunctions1[itarget].Get(0);
                    timein = 0;
                }
                else if (dfunctions2[itarget] != null && dfunctions2[itarget].Get(0) != null)
                {
                    animation = dfunctions2[itarget].Get(0);
                    timein = 0;
                }
            }
            prevbutton = Mouse.GetState().RightButton;
        }

        // "theline" is actual path we are taking, opposite of the path we rode in on
        // "otherline" is the line we rode in on, we are guaranteed to be on the "top" of this
        // TODO: wait how do we deal with vertical lines again?? not that it's ever ambiguous in our current level...
        private void DoAThing2(Line2D theline, Line2D otherline, DiscreteFunction dfunction, DiscreteFunction[] newdfunctions, DiscreteFunction[] newdfunctions1, DiscreteFunction[] newdfunctions2, int i, SavableTerrain2D mainTerrainClone)
        {
            if (dfunction != null)
            {
                Vector2D thepoint = mainTerrainClone.points[i];
                Vector2D vector1 = otherline.AsVector2DWithEnd(thepoint).Normalize(1);
                Vector2D vector2 = theline.AsVector2DWithEnd(thepoint).Normalize(1) * -1;
                bool bouncejump = vector1.x*vector2.x<0; // you will bounce jump whenver you go left to right, or right to left (we'll say not if either is vertical)
                bool justjump=false; // this will happen if you only have one line (which we haven't coded for), or when the path curves down
                if (vector1.x > 0 && vector1.CrossProduct(vector2)<0)
                {
                    justjump = true;
                }
                if (vector1.x < 0 && vector1.CrossProduct(vector2) > 0)
                {
                    justjump = true;
                }
                if (bouncejump && justjump)
                {
                    throw new NotImplementedException();
                }
                int thei = mainTerrainClone.points.IndexOf(theline.OtherP(thepoint));
                double thea1 = apg + theline.AsVector2D().GravityMultiplier() * ay;
                double thea2 = -apg + theline.AsVector2D().GravityMultiplier() * ay;
                var attachedattached = mainTerrainClone.Attached(mainTerrainClone.points[thei]);
                foreach (var time in dfunction.times.ToList()) // suddenly this is complaining about modification with the addition of returnfaster/returnslower?? using tolist to quiet it
                {
                    //if (!bouncejump && !justjump)// TODO: hardcoding conditions because we've messed up somehow
                    {
                        if (attachedattached[0] == theline) // weird logic
                        {
                            DoAThing(time, theline, thepoint, thei, newdfunctions2, new StraightEq(thea1, thea2, theline.Length));
                        }
                        else
                        {
                            DoAThing(time, theline, thepoint, thei, newdfunctions1, new StraightEq(thea1, thea2, theline.Length));
                        }
                        DoAThing(time, theline, thepoint, i, newdfunctions, new ReturnFasterEq(thea1, thea2, theline.Length)); // because you return, you are always returning to the same point using the other function list
                        DoAThing(time, theline, thepoint, i, newdfunctions, new ReturnSlowerEq(thea1, thea2, theline.Length));
                    }
                    //if (justjump)// TODO: hardcoding conditions because we've messed up somehow
                    {
                        // TODO: we're going to forget about 0-speed for now, where you're allowed to hit one of the lines you're flying off
                        Line2D linetoavoid = otherline; // no collision at all possible with this
                        Line2D linetomostlyavoid = theline; // only 1 of 2 possible collisions with this

                        //fullinfos.Add(AdvGeomLogic.FullJumpInfo(mainTerrain.lines, line, lines, SelectedPoint.AsVector2D(), velocity, player.ay));
                        var jumpinfo = AdvGeomLogic.FullJumpInfo(mainTerrainClone.lines, otherline, new Line2D[] { theline }, thepoint, vector1 * (time.Key / PREC), 9.8);
                        if (jumpinfo != null && jumpinfo.collider!=null)
                        {
                            var relv = jumpinfo.collider.PV(jumpinfo.anim.EndV());
                            var actualv = relv * jumpinfo.collider.Length;
                            if (relv > 0)
                            {
                                int landi = mainTerrainClone.points.IndexOf(jumpinfo.collider.p2);
                                double landa1 = apg + jumpinfo.collider.AsVector2D().GravityMultiplier() * ay;
                                double landa2 = -apg + jumpinfo.collider.AsVector2D().GravityMultiplier() * ay;
                                ISpeedTimeFunction path = new StraightEq(landa1, landa2, (1 - jumpinfo.collidep) * jumpinfo.collider.Length);
                                Bounds vbounds = path.EBound(actualv / PREC);
                                if (vbounds != null)
                                {
                                    int low = (int)Math.Ceiling(vbounds.low * PREC);
                                    int high = (int)(vbounds.high * PREC);
                                    Vector2D vector = jumpinfo.collider.AsVector2D().Normalize(1);
                                    for (int j = low; j <= high; j++)
                                    {
                                        IAnimation pathanim = path.Animate(actualv / PREC, j / PREC);
                                        if (pathanim == null) continue; // this has happened, I think
                                        IAnimation2D animation = new AnimationFrom1D(jumpinfo.anim.EndPos(), vector, pathanim);
                                        animation = time.Value + jumpinfo.anim + animation;
                                        if (newdfunctions[landi] == null) newdfunctions[landi] = new DiscreteFunction();
                                        if (!newdfunctions[landi].times.ContainsKey(Math.Abs(j)) || animation.Time() < newdfunctions[landi].times[Math.Abs(j)].Time())
                                        {
                                            newdfunctions[landi].Put(Math.Abs(j), animation);
                                        }
                                    }
                                }
                                // TODO: ignoring return moves for jumping for now
                            }
                            else
                            {
                                int landi = mainTerrainClone.points.IndexOf(jumpinfo.collider.p1);
                                double landa1 = apg + jumpinfo.collider.AsVector2D().GravityMultiplier() * ay;
                                double landa2 = -apg + jumpinfo.collider.AsVector2D().GravityMultiplier() * ay;
                                ISpeedTimeFunction path = new StraightEq(landa1, landa2, jumpinfo.collidep * jumpinfo.collider.Length);
                                Bounds vbounds = path.EBound(-actualv / PREC);
                                if (vbounds != null)
                                {
                                    int low = (int)Math.Ceiling(vbounds.low * PREC);
                                    int high = (int)(vbounds.high * PREC);
                                    Vector2D vector = jumpinfo.collider.AsVector2D().Normalize(1) * -1;
                                    for (int j = low; j <= high; j++)
                                    {
                                        IAnimation pathanim = path.Animate(-actualv / PREC, j / PREC);
                                        if (pathanim == null) continue; // this has happened, I think
                                        IAnimation2D animation = new AnimationFrom1D(jumpinfo.anim.EndPos(), vector, pathanim);
                                        animation = time.Value + jumpinfo.anim + animation;
                                        if (newdfunctions[landi] == null) newdfunctions[landi] = new DiscreteFunction();
                                        if (!newdfunctions[landi].times.ContainsKey(Math.Abs(j)) || animation.Time() < newdfunctions[landi].times[Math.Abs(j)].Time())
                                        {
                                            newdfunctions[landi].Put(Math.Abs(j), animation);
                                        }
                                    }
                                }
                                // TODO: ignoring return moves for jumping for now
                            }
                        }
                    }
                }
            }
        }

        private void DoAThing(KeyValuePair<int, IAnimation2D> time, Line2D theline, Vector2D thepoint, int thei, DiscreteFunction[] newdfunctions, ISpeedTimeFunction path)
        {
            Bounds vbounds = path.EBound(time.Key / PREC);
            if (vbounds == null) return;
            int low = (int)Math.Ceiling(vbounds.low * PREC);
            int high = (int)(vbounds.high * PREC);
            Vector2D vector = theline.AsVector2DWithEnd(thepoint).Normalize(1) * -1;
            for (int j = low; j <= high; j++)
            {
                IAnimation pathanim = path.Animate(time.Key / PREC, j / PREC);
                if (pathanim == null) continue; // this has happened, I think
                IAnimation2D animation = new AnimationFrom1D(thepoint, vector, pathanim);
                animation = time.Value + animation;
                if (newdfunctions[thei] == null) newdfunctions[thei] = new DiscreteFunction();
                if (!newdfunctions[thei].times.ContainsKey(Math.Abs(j)) || animation.Time() < newdfunctions[thei].times[Math.Abs(j)].Time())
                {
                    newdfunctions[thei].Put(Math.Abs(j), animation);
                }
            }
        }

        private DiscreteFunction InitializeDFunction(HumanPlayer player, SavableTerrain2D mainTerrain, int i1, int i2, int l1) // i1 represents p1
        {
            Vector2D startpoint = player.attached.At(player.p);
            mainTerrain.points.Add(startpoint);
            mainTerrain.lines.RemoveAt(l1);
            if (player.vp > 0) // you're traveling towards i2, so make that segment have the lower index
            {
                mainTerrain.lines.Add(new Line2D(startpoint, mainTerrain.points[i2]));
                mainTerrain.lines.Add(new Line2D(mainTerrain.points[i1], startpoint));
            }
            else
            {
                mainTerrain.lines.Add(new Line2D(mainTerrain.points[i1], startpoint));
                mainTerrain.lines.Add(new Line2D(startpoint, mainTerrain.points[i2]));
            }
            DiscreteFunction[] dfunctions = new DiscreteFunction[mainTerrain.points.Count];
            DiscreteFunction startfunc = new DiscreteFunction();
            dfunctions[mainTerrain.points.Count - 1] = startfunc;
            // TODO: figure out gravity and accelerations and stuff
            // TODO: do the othe returnfaster/returnslower
            double v = player.vp * player.attached.Length;
            startfunc.Put((int)Math.Round(Math.Abs(v * PREC)), new EmptyAnimation2D());
            return startfunc;
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
}
