using System;

namespace SistemaDeAnimaisMVC.Models
{
    public class ReturnLoginModel
    {
        public string Msg { get; set; }
        public bool Status { get; set; }
        public bool Authenticated { get; set; }
        public DateTime Expiration { get; set; }
        public string Token { get; set; }
    }
}
