using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HockeyPlayerDatabase.Interfaces;

namespace HockeyPlayerDatabase
{
    public class Player : IPlayer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string TitleBefore { get; set; }
        public int YearOfBirth { get; set; }
        public int KrpId { get; set; }
        public AgeCategory? AgeCategory { get; set; }
        public int? ClubId { get; set; }

        public Player(int id, string firstName, string lastName, string titleBefore, int yearOfBirth, int krpId, AgeCategory? ageCategory, int? clubId)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            FullName = firstName + " " + lastName;
            TitleBefore = titleBefore;
            YearOfBirth = yearOfBirth;
            KrpId = krpId;
            AgeCategory = ageCategory;
            ClubId = clubId;
        }

        public Player()
        {
        }

        public override string ToString()
        {
            return "ID: " + Id + " ; " + "Full name: " + FullName + " ; " + "Title: " + TitleBefore + " ; " + "Year: " +
                   YearOfBirth + " ; " + "KrpId: " + KrpId + " ; " + "AgeCat: " + AgeCategory.ToString() + " ; " +
                   "ClubId: " + ClubId;
        }
    }
}
