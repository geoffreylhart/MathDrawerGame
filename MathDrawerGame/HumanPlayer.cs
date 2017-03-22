using MathDrawerGame.Geom;
using MathDrawerGame.LevelEditor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathDrawerGame
{
    class HumanPlayer
    {
        public class CWCache
        {
            public bool oncw = false;
            public double time = -1; //expiretime

            public CWCache(bool oncw, double time)
            {
                this.oncw = oncw;
                this.time = time;
            }
        }
        Dictionary<IntLine, CWCache> cachedCW = new Dictionary<IntLine, CWCache>();
        public string errortext = "nan";
        double x = 18.5;
        double y = 5;
        double apg = 30; // meters per second squared (4.47=performance car, 12.2=fastest exotic car)
        public double ay = 9.8; // gravity, meters per second squared
        double vx = 2;
        double vy = 0;
        public double p = 0;
        public double vp = 0; // in relative terms
        public IntLine attached = null;
        bool oncw = false; // needed for vertical lines mostly
        double totaltime = 0;
        double cachetime = 1;
        ButtonState prevbutton = ButtonState.Released;
        Vector2D dragstart = null;
        internal void Update(double dt, SavableTerrain terrain, Vector2D relativemouse)
        {
            if (prevbutton==ButtonState.Released && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                dragstart = relativemouse;
            }
            if (prevbutton == ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                Vector2D dragend = relativemouse;
                Vector2D velocity = dragend - dragstart;
                attached = null;
                x = dragstart.x;
                y = dragstart.y;
                dragstart = null;
                vx = velocity.x;
                vy = velocity.y;
            }
            prevbutton = Mouse.GetState().LeftButton;
            if (attached == null)
            {
                SimulateFree(dt, terrain);
            }
            else
            {
                SimulateNotFree(dt, terrain);
            }
        }

        private void SimulateFree(double dt, SavableTerrain terrain)
        {
            double mint = dt;
            double thisp = 0;
            IntLine targetline = null;
            foreach (IntLine line in terrain.lines)
            {
                if (line.Vertical) // infinite slope
                {
                    if (vx == 0) continue;
                    double t = (line.p1.x - x) / vx;
                    if (MathHelper.Between(t, 0.0001, mint)) // TODO: this here and elsewhere is a hack to avoid stack overflow
                    {
                        double newy = 0.5 * ay * t * t + vy * t + y;
                        if (MathHelper.Between(newy, line.p1.y, line.p2.y))
                        {
                            double tvx = vx;
                            double tvy = vy + ay * t;
                            double thistotalv = Math.Sqrt(tvx * tvx + tvy * tvy);
                            double thiscrossp = (tvx * line.DY - tvy * line.DX) / (thistotalv * line.Length);
                            bool thisoncw = thiscrossp > 0;
                            if(cachedCW.ContainsKey(line) && cachedCW[line].time>totaltime+t){
                                if (cachedCW[line].oncw != thisoncw) continue;
                            }
                            mint = t;
                            targetline = line;
                            thisp = line.PFromY(newy);
                        }
                    }
                }
                else
                {
                    // nx = x+vx*t
                    // ny = y+0.5*ay*t*t+vy*t
                    // ny=line.M*nx+line.B
                    // y+0.5*ay*t*t+vy*t=line.M*(x+vx*t)+line.B
                    double a = 0.5 * ay;
                    double b = vy - line.M * vx;
                    double c = y - line.B - line.M * x;
                    if (b * b - 4 * a * c < 0) continue;
                    double t1 = (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
                    double t2 = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
                    foreach (double t in new double[] { t1, t2 })
                    {
                        if (MathHelper.Between(t, 0.0001, mint)) // TODO: this here and elsewhere is a hack to avoid stack overflow
                        {
                            double newx = x + t * vx;
                            if (MathHelper.Between(newx, line.p1.x, line.p2.x))
                            {
                                double tvx = vx;
                                double tvy = vy + ay * t;
                                double thistotalv = Math.Sqrt(tvx * tvx + tvy * tvy);
                                double thiscrossp = (tvx * line.DY - tvy * line.DX) / (thistotalv * line.Length);
                                bool thisoncw = thiscrossp > 0;
                                if (cachedCW.ContainsKey(line) && cachedCW[line].time > totaltime + t)
                                {
                                    if (cachedCW[line].oncw != thisoncw) continue;
                                }
                                mint = t;
                                targetline = line;
                                thisp = line.PFromX(newx);
                                break;
                            }
                        }
                    }
                }
            }
            if (targetline != null)
            {
                x += vx * mint;
                y += 0.5 * ay * mint * mint + vy * mint;
                vy += ay * mint;
                attached = targetline;
                p = thisp;
                double totalv = Math.Sqrt(vx * vx + vy * vy);
                vp = totalv/attached.Length;
                // note, I think this works regardless of which direction the line is facing
                double dotp = (vx * attached.DX + vy * attached.DY) / (totalv * attached.Length);
                double crossp = (vx * attached.DY - vy * attached.DX) / (totalv * attached.Length);
                oncw = crossp > 0;
                cachedCW[targetline] = new CWCache(oncw, totaltime+mint+cachetime);
                vp *= dotp;
                // TODO: we are still attaching to some ceilings (could this be due to faulty error protection?)
                if ((oncw && targetline.DX > 0) || (!oncw && targetline.DX < 0))
                {
                    // hitting ceiling
                    vx = vp * attached.DX;
                    vy = vp * attached.DY;
                    attached = null;
                    totaltime += mint;
                    SimulateFree(dt - mint, terrain);
                }
                else
                {
                    totaltime += mint;
                    SimulateNotFree(dt - mint, terrain);
                }
            }
            else
            {
                x += vx * dt;
                y += 0.5 * ay * dt * dt + vy * dt;
                vy += ay * dt;
            }
        }

        private void SimulateNotFree(double dt, SavableTerrain terrain)
        {
            double mint = dt;
            // TODO: base acceleration on whether youre cw or not, since vertical walls have trouble
            double ap = ay * attached.DY / (attached.Length * attached.Length);
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                ap += apg / attached.Length * ((attached.DX > 0) ? 1 : -1);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                ap -= apg / attached.Length * ((attached.DX > 0) ? 1 : -1);
            }
            IntPoint targetpoint = null;
            if (ap == 0)
            {
                //np=p+vp*t
                double t1 = -p / vp;
                double t2 = (1 - p) / vp;
                if (MathHelper.Between(t1, 0, mint))
                {
                    mint = t1;
                    targetpoint = attached.p1;
                }
                if (MathHelper.Between(t2, 0, mint))
                {
                    mint = t2;
                    targetpoint = attached.p2;
                }
            }
            else
            {
                //np=p+0.5*ap*t*t+vp*t
                double a = 0.5 * ap;
                double b = vp;
                double c0 = p;
                double c1 = p - 1;
                if (b * b - 4 * a * c0 >= 0)
                {
                    double t1 = (-b - Math.Sqrt(b * b - 4 * a * c0)) / (2 * a);
                    double t2 = (-b + Math.Sqrt(b * b - 4 * a * c0)) / (2 * a);
                    if (MathHelper.Between(t1, 0, mint))
                    {
                        mint = t1;
                        targetpoint = attached.p1;
                    }
                    if (MathHelper.Between(t2, 0, mint))
                    {
                        mint = t2;
                        targetpoint = attached.p1;
                    }
                }
                if (b * b - 4 * a * c1 >= 0)
                {
                    double t1 = (-b - Math.Sqrt(b * b - 4 * a * c1)) / (2 * a);
                    double t2 = (-b + Math.Sqrt(b * b - 4 * a * c1)) / (2 * a);
                    if (MathHelper.Between(t1, 0, mint))
                    {
                        mint = t1;
                        targetpoint = attached.p2;
                    }
                    if (MathHelper.Between(t2, 0, mint))
                    {
                        mint = t2;
                        targetpoint = attached.p2;
                    }
                }
            }
            if (targetpoint == null)
            {
                p += 0.5 * ap * dt * dt + vp * dt;
                vp += ap * dt;
                totaltime += dt;
                x = attached.XAt(p); // just for visualization
                y = attached.YAt(p);
                cachedCW[attached] = new CWCache(oncw, totaltime + cachetime);
            }
            else
            {
                IntLine nextline = terrain.OtherLine(targetpoint, attached);
                // we assume we are on top of the line at this point
                double dp = attached.DotProduct(nextline);
                errortext = attached.DX + ": " + attached.DY + ": " + oncw;
                bool hittingceil;
                if (attached.DX == 0)
                {
                    hittingceil=oncw==(nextline.DX>0);
                }else{
                    hittingceil = attached.DX * nextline.DX < 0;
                }
                if (targetpoint == attached.p1){
                    dp *= -1;
                    hittingceil = !hittingceil;
                }
                if (targetpoint == nextline.p2){
                    dp *= -1;
                    hittingceil = !hittingceil;
                }
                if (hittingceil)
                {
                    vp += mint * ap;
                    vp *= attached.Length / nextline.Length;
                    vp *= Math.Abs(dp);
                    if ((targetpoint == nextline.p1 && targetpoint == attached.p1) || (targetpoint == nextline.p2 && targetpoint == attached.p2))
                    {
                        vp *= -1;
                    }
                    vx = vp * nextline.DX;
                    vy = vp * nextline.DY;
                    vp = 0; //doesnt matter
                    x = targetpoint.x;
                    y = targetpoint.y;
                    cachedCW[attached] = new CWCache(oncw, totaltime + mint + cachetime);
                    if ((targetpoint == nextline.p1 && targetpoint == attached.p1) || (targetpoint == nextline.p2 && targetpoint == attached.p2))
                    {
                        cachedCW[nextline] = new CWCache(!oncw, totaltime + mint + cachetime);
                    }
                    else
                    {
                        cachedCW[nextline] = new CWCache(oncw, totaltime + mint + cachetime);
                    }
                    attached = null;
                    totaltime += mint;
                    SimulateFree(dt - mint, terrain);
                    return;
                }
                if (dp > 0)
                {
                    if (targetpoint == nextline.p1)
                    {
                        p = 0.001; // TODO: replace this with actual error protection
                    }
                    else
                    {
                        p = 0.999;
                    }
                    vp += mint * ap;
                    vp *= attached.Length / nextline.Length;
                    vp *= Math.Abs(dp);

                    cachedCW[attached] = new CWCache(oncw, totaltime + mint + cachetime);
                    if ((targetpoint == nextline.p1 && targetpoint == attached.p1) || (targetpoint == nextline.p2 && targetpoint == attached.p2))
                    {
                        vp *= -1;
                        oncw = !oncw;
                    }
                    attached = nextline;
                    cachedCW[attached] = new CWCache(oncw, totaltime + mint + cachetime);
                    totaltime += mint;
                    SimulateNotFree(dt - mint, terrain);
                }
                else
                {
                    vp += mint * ap;
                    vx = vp * attached.DX;
                    vy = vp * attached.DY;
                    vp = 0; //doesnt matter
                    cachedCW[attached] = new CWCache(oncw, totaltime + mint + cachetime);
                    if ((targetpoint == nextline.p1 && targetpoint == attached.p1) || (targetpoint == nextline.p2 && targetpoint == attached.p2))
                    {
                        cachedCW[nextline] = new CWCache(!oncw, totaltime + mint + cachetime);
                    }
                    else
                    {
                        cachedCW[nextline] = new CWCache(oncw, totaltime + mint + cachetime);
                    }
                    attached = null;
                    totaltime += mint;
                    SimulateFree(dt - mint, terrain);
                }
            }
        }

        internal void Draw(BasicEffect basicEffect, GraphicsDevice GraphicsDevice)
        {
            basicEffect.DrawPoint(GraphicsDevice, x, y, Color.Green);
            if (dragstart != null)
            {
                Vector2D dragend = basicEffect.GetMouseRelative(GraphicsDevice);
                Vector2D velocity = dragend-dragstart;
                ParabolaSegment newpath = ParabolaSegment.FromBasics(dragstart, velocity, ay, 5);
                basicEffect.DrawParabola(GraphicsDevice, newpath);
            }
        }
    }
}
