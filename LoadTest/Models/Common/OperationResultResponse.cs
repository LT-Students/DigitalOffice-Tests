using DigitalOffice.LoadTesting.Models.Enums;
using System.Collections.Generic;

namespace DigitalOffice.LoadTesting.Models.Responses.Templates
{
    public class OperationResultResponse<T>
    {
        public T Body { get; set; }
        public OperationResultStatusType Status { get; set; }
        public List<string> Errors { get; set; } = new();

        public OperationResultResponse(
            T body = default,
            OperationResultStatusType status = default,
            List<string> errors = default)
        {
            Body = body;
            Status = status;
            Errors = errors ?? new();
        }
    }
}
