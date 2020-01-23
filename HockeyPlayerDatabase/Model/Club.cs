using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HockeyPlayerDatabase.Interfaces;

namespace HockeyPlayerDatabase
{
    public class Club : IClub
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Url { get; set; }
        public ICollection<Player> Players { get; set; }

        public Club(int id, string name, string address, string url)
        {
            Id = id;
            Name = name;
            Address = address;
            Url = url;
            Players = new List<Player>();
        }

        public Club()
        {
            //throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "ID: " + Id + " ; " + "Name: " + Name + " ; " + "Address: " + Address + " ; " + "Url: " + Url + " ; " + "Players count: " + Players.Count;
        }
    }
}
