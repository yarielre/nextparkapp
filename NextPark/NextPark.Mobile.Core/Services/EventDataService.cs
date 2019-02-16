using NextPark.Mobile.Core.Settings;
using NextPark.Models;

namespace NextPark.Mobile.Services.Data
{
    public class EventDataService : BaseDataService<EventModel>
    {
        public EventDataService(IApiService apiService) : base(apiService, ApiSettings.EventsEndPoint)
        {
        }
    }
}
