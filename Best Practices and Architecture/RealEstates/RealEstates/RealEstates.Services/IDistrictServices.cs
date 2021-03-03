using RealEstates.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstates.Services
{
    public interface IDistrictServices
    {
        IEnumerable<DistrictViewModel> GetTopDistrictsByAveragePrice(int count = 10);

        IEnumerable<DistrictViewModel> GetTopDistrictsByNumberOfProperties(int count = 10);

    }
}
