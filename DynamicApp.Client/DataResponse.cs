namespace DynamicApp.Client
{
    public class DataResponse<T> : Response where T : IResponseDto
    {
        public T Data { get; set; }
        public new void ConvertFromUTC() => Data?.ConvertFromUTC();
    }
}
