namespace Interfaces {
    public class BaseResponse {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public object Data { get; set; } = null;
    }
    public class ServiceResponse<T> : BaseResponse {
        public new T Data { get; set; }

        public ServiceResponse() {
        }
    }
}
