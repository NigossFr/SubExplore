using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using SubExplore.Models;
using SubExplore.ViewModels.Base;

namespace SubExplore.Models
{
    public class SpotMarker : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Title { get; set; }
        public SpotType Type { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public string IconPath { get; set; }

        public string DisplayDifficulty => DifficultyLevel.ToDisplayText();

        public Location Location => new(Latitude, Longitude);

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
