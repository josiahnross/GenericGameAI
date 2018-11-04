using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuralNetTreeStuffViewer
{
    public class GameButtonArgs<T> : EventArgs
    {
        public T Info { get; set; }
        public GameButtonArgs(T info)
        {
            Info = info;
        }
    }
    public class GameButton<T> : Button
    {
        public new event EventHandler<GameButtonArgs<T>> Click;
        public T Info { get; set; }
        public GameButton(T info)
        {
            Info = info;

            base.Click += HandleButtonClicked;
        }

        private void GameButton_Load(object sender, EventArgs e)
        {
        }
        private void HandleButtonClicked(object sender, EventArgs e)
        {
            this.OnButtonClicked(new GameButtonArgs<T>(Info));
        }
        protected virtual void OnButtonClicked(GameButtonArgs<T> e)
        {
            EventHandler<GameButtonArgs<T>> handler = this.Click;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
