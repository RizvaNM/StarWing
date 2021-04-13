﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using StarWing.Framework.Primitives;

namespace StarWing.Framework.Input
{
    public class Mouse : IMouse
    {
        // Better to store it in integer
        private List<MouseButtons> _pressed;
        private MouseButtons _justPressed;
        public Point Position { get; private set; }

        public MouseStatus Status
        {
            get
            {
                var justPressed = _justPressed;
                _justPressed = (int) MouseButtons.None;
                return new MouseStatus(Position, _pressed, justPressed);
            }
        }

        /// <param name="form">Form to listen input from</param>
        public Mouse(IPressableManipulator<MouseEventArgs> pressableManipulator, IMovableManipulator<MouseEventArgs> movableManipulator)
        {
            if (pressableManipulator == null)
            {
                var exception = new ArgumentNullException(nameof(pressableManipulator));
                Log.Error("Pressable manipulator in mouse class was null", exception);
                throw exception;
            }

            if (movableManipulator == null)
            {
                var exception = new ArgumentNullException(nameof(movableManipulator));
                Log.Error("Movable manipulator in mouse class was null", exception);
                throw exception;
            }

            pressableManipulator.KeyDown += UpdateOnMouseDown;
            pressableManipulator.KeyUp += UpdateOnMouseUp;
            movableManipulator.Move += UpdateMousePosition;

            _justPressed = new();
            _pressed = new();
            Position = Vector2D.Zero;
        }

        private void UpdateMousePosition(object? sender, MouseEventArgs e)
        {
            Position = e.Location;
        }

        private void UpdateOnMouseDown(object? sender, MouseEventArgs e)
        {
            UpdateMousePosition(sender, e);
            var button = e.Button;

            // Pressed first time
            if (!_pressed.Contains(button))
            {
                _justPressed = button;
                _pressed.Add(button);
            }
            else
            {
                _justPressed = MouseButtons.None;
            }
        }

        private void UpdateOnMouseUp(object? sender, MouseEventArgs e)
        {
            UpdateMousePosition(sender, e);
            _pressed.Remove(e.Button);
        }
    }
}