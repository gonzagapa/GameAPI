using GameStore.Models;

namespace GameStore.Dtos
{
    public record PageResponseOffsetDto<T> where T:class
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }
        public List<T> Data { get; set; }

        public PageResponseOffsetDto(int pageNumber, int pageSize, int total, List<T> data)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = total;
            TotalPages = (int)Math.Ceiling((decimal)TotalRecords/PageSize);
            Data = data;
        }
    }
}