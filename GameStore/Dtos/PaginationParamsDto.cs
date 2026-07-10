namespace GameStore.Dtos
{
    public record PaginationParamsDto(int PageNumber, int PageSize)
    {
        private const int MAX_PAGE_SIZE = 30;
        private const int MIN_PAGE_SIZE = 5;

        //Aprender sobre BindAsync
         public static ValueTask<PaginationParamsDto?> BindAsync(HttpContext context, System.Reflection.ParameterInfo parameter)
        {
            var query = context.Request.Query;

            if(!int.TryParse(query["pageNumber"], out var pageNumber))
            {
                pageNumber = 1;
            }

            if(!int.TryParse(query["pageSize"], out var pageSize))
            {
                pageSize = 5;
            }

            pageSize = pageSize > MAX_PAGE_SIZE || pageSize <= 0 ? MIN_PAGE_SIZE : pageSize;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;

            return ValueTask.FromResult<PaginationParamsDto?>(new PaginationParamsDto(pageNumber, pageSize));
        }
    }
}