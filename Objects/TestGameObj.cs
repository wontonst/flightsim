﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace itp380.Objects
{
    public class TestGameObj : GameObject
    {
        public TestGameObj(Game game) :
            base(game)
        {
            m_ModelName = "Miner/Miner";
        }
    }
}
