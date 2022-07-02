using ParkyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository.IRepository
{
    public interface INationalParkRepository
    {
        ICollection<Models.NationalPark> GetNationalParks();
        Models.NationalPark GetNationalPark(int nationalParkId);
        bool NationalParkExists(string name);
        bool NationalParkExists(int id);
        bool CreateNationalPark(Models.NationalPark nationalPark);
        bool UpdateNationalPark(Models.NationalPark nationalPark);
        bool DeleteNationalPark(Models.NationalPark nationalPark);
        bool Save();

    }
}
