using Cysharp.Threading.Tasks;
using Game.Core;
using UnityEngine;

namespace Managers
{
    public class DisplayManager : MonoBehaviour
    {
        [Header("Turn Transition")] 
        [SerializeField] private Material playerMaterial;
        [SerializeField] private float duration = 0.5f;
        
        private static readonly int PlayerColorID = Shader.PropertyToID("_PlayerColor");
        private static readonly int OldColorID = Shader.PropertyToID("_OldColor");
        private static readonly int TransitionID = Shader.PropertyToID("_Transition");
        
        public async UniTask DisplayCurrentPlayerTurn(IPlayer playerManagerCurrentPlayer)
        {
            Color oldColor = playerMaterial.GetColor(PlayerColorID);
            Color playerColor = playerManagerCurrentPlayer.GetPlayerInformation().SharedPlayerMaterial.GetColor(PlayerColorID);
            playerMaterial.SetColor(OldColorID, oldColor);
            playerMaterial.SetColor(PlayerColorID, playerColor);
            
            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                playerMaterial.SetFloat(TransitionID, t/duration);
                await UniTask.Yield();
            }
            playerMaterial.SetFloat(TransitionID, 1);
        }
        
        public async UniTaskVoid PlacePieceEffect(PieceData placementData)
        {
            Debug.LogWarning("Implement PlacePieceEffect effect, playing an animation");
        }
        public async UniTaskVoid InvalidPieceEffect(PieceData placementData)
        {
            Debug.LogWarning("Implement InvalidPieceEffect effect, playing an animation");
        }
        public async UniTaskVoid MissedPieceEffect(PieceData placementData)
        {
            Debug.LogWarning("Implement MissedPieceEffect effect, playing an animation");
        }

        public async UniTask HandleTie()
        {
            Debug.Log("GAME ENDED WITH A TIE");
        }

        public async UniTask AwardWin(IPlayer[] winner)
        {
            Debug.Log("SOMEONE HAS WON");
            foreach (var x in winner)
            {
                Debug.Log("SOMEONE HAS WON: " + x.GetPlayerInformation().TeamNumber + " --> " +
                          (x as MonoBehaviour)?.name);
            }

            float t = 0;
            const float delay = 15;
            Color oldColor = playerMaterial.GetColor(PlayerColorID);
            playerMaterial.SetColor(OldColorID, oldColor);
            while (t < delay)
            {
                t += Time.deltaTime;
                playerMaterial.SetFloat(TransitionID, (Mathf.Sin(t/delay) + 1) * 0.5f);
                await UniTask.Yield();
            }
        }
    }
}