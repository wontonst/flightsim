using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace itp380.Graphics
{
    public class GameMessages
    {
        private string msg;
        public GameMessages()
        {

        }
        public string getMessage()
        {
            return this.msg == null ? "" : this.msg;
        }
        public void setMessage(string msg)
        {
            this.msg = msg;
        }
    }
}
