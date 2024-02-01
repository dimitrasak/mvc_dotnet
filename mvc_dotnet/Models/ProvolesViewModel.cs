using X.PagedList;

namespace mvc_dotnet.Models
{
    public class ProvolesViewModel
    {
        public IPagedList<Provole> Provoles { get; set; }
        public List<int> AvailableSeatsList { get; set; }
    }

}
