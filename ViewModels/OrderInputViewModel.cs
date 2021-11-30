using System.Collections.Generic;
using OpticsStore.Models;

namespace OpticsStore.ViewModels
{
    public class OrderInputViewModel
    {
        public Order Order { get; set; }
        public IEnumerable<GlassesFrame> GlassesFrames { get; set; }
        public IEnumerable<Clinic> Clinics { get; set; }
    }
}