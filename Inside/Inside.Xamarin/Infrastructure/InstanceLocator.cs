using Inside.Xamarin.ViewModels;

namespace Inside.Xamarin.Infrastructure
{
   public class InstanceLocator
    {
        public MainViewModel Main { get; set; }

        public InstanceLocator()
        {
            this.Main = new MainViewModel();
        }
    }
}
