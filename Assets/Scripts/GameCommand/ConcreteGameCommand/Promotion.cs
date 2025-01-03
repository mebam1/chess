using Photon.Pun;
using UnityEngine;
namespace Command
{
    public class Promotion : IGameCommand
    {
        public event IGameCommand.DoHandler OnDo;
        int toX, toY;
        int fromX, fromY;
        BaseMovement target;
        BaseMovement promotioned;
        public IGameCommand Last => this;

        public Promotion(BaseMovement target, int toX, int toY, int fromX, int fromY)
        {
            this.toX = toX;
            this.toY = toY;
            this.fromX = fromX;
            this.fromY = fromY;
            this.target = target;
        }

        public string Info => $"[Promotion]";

        public void Do()
        {
            var promotionTab = ChessVisualizer.Instance.PromotionTab;
            promotionTab.OnSelected -= PromotionTab_OnSelected;
            promotionTab.OnSelected += PromotionTab_OnSelected;
            if (PhotonNetwork.IsMasterClient && (target.Color == BaseMovement.PieceColor.BLACK)) return;
            if (!PhotonNetwork.IsMasterClient && (target.Color == BaseMovement.PieceColor.WHITE)) return;
            promotionTab.gameObject.SetActive(true);
            promotionTab.SetColor(target.Color);
        }

        void PromotionTab_OnSelected(BaseMovement createdInstance)
        {
            promotioned = createdInstance;
            var promotionTab = ChessVisualizer.Instance.PromotionTab;
            promotionTab.OnSelected -= PromotionTab_OnSelected;
            promotionTab.gameObject.SetActive(false);

            // insert created instance to chess board. create visual to show instance.
            createdInstance.x = toX;
            createdInstance.y = toY;
            var visualPiece = ChessVisualizer.Instance.GetPieceSet(target.Color);
            var friendly = ChessRule.Instance.GetPieceSet(target.Color);
            friendly.Remove(target);
            target.RemoveVisual();
            friendly.Add(createdInstance);
            visualPiece.BindVisualizer(createdInstance);
            OnDo?.Invoke();
        }

        public void UnDo()
        {
            throw new System.NotImplementedException();
        }
    }
}
