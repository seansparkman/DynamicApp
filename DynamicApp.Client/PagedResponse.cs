namespace DynamicApp.Client
{
    public class PagedResponse<T> : ListResponse<T> where T : IResponseDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
