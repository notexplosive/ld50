using System;
using Machina.Data;
using Machina.Engine;
using MachinaDesktop;
using Microsoft.Xna.Framework;

namespace LD50
{
    public static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            MachinaBootstrap.Run(
                new GameSpecification("LD50 Game - NotExplosive.net", args, new GameSettings(new Point(900, 1600))),
                new Ld50Cartridge(), ".");
        }
    }
}
