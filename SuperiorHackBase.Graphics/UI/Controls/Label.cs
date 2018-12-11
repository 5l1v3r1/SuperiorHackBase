﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperiorHackBase.Core.Maths;
using SuperiorHackBase.Graphics.UI.Painting;

namespace SuperiorHackBase.Graphics.UI.Controls
{
    public class Label : Control
    {
        public Label()
        {
            AutoSize = true;
        }

        public override void Draw(IRenderer renderer)
        {
            if (renderer == null)
                return;

            if (RecalculateSize)
            {
                RecalculateSize = false;
                Size = renderer.MeasureString(Text, Font);
            }

            var absPos = AbsolutePosition;
            var bounds = new Rectangle(absPos.X, absPos.Y, Width, Height);
            if (DrawBackground)
                renderer.FillRectangle(bounds, BackgroundColor);
            if (DrawBorder)
                renderer.DrawRectangle(bounds, BorderColor, 1f);
            if (DrawBackground)
                renderer.DrawString(bounds, Text,Font,ForegroundColor);
        }
    }
}
