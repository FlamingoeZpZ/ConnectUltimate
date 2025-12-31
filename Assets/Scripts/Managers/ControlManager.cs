using Game.Core;
using Game.Players;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    //TODO: Fix bug that allows spam click spawning... I think it's just that though... A fucked editor only bug.
    internal static class ControlManager
    {
        private static Human _inputPlayer;
        private static readonly GameControls Controls;

        static ControlManager()
        {
            Controls = new GameControls();
                
            Controls.Player.TapRelease.performed += ctx => _inputPlayer!.PlacePiecePlayer(ctx.ReadValue<Vector2>());
            Controls.Player.Release.performed += _ => _inputPlayer!.TryDropPiecePlayer();
            Controls.Player.MousePos.performed += ctx => _inputPlayer!.HandlePiecePlayer(ctx.ReadValue<Vector2>());

            Controls.Disable();
        }

        internal static void SetCurrentTurn(IPlayer player)
        {
            _inputPlayer = player as Human;
            if (_inputPlayer != null) EnableGameControls();
            else DisableGameControls();
        }

        internal static void DisableGameControls() => Controls.Player.Disable();
        internal static void EnableGameControls()
        {
            Controls.Player.Enable();
        }

        internal static void EnterUIControl()
        {
            DisableGameControls();
            Controls.UI.Enable();
        }

        internal static void ExitUIControl()
        {
            SetCurrentTurn(_inputPlayer);
            Controls.UI.Disable();
        }
    }
}