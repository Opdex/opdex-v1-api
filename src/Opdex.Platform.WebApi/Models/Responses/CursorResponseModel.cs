namespace Opdex.Platform.WebApi.Models.Responses
{
    public class CursorResponseModel
    {
        /// <summary>
        /// Cursor for the next page.
        /// </summary>
        /// <example>ZGlyZWN0aW9uOkRFU0M7bGltaXQ6MTA7cGFnaW5nOkZvcndhcmQ7d2FsbGV0Ojtwb2ludGVyOk5qUTA7</example>
        public string Next { get; set; }

        /// <summary>
        /// Cursor for the previous page.
        /// </summary>
        /// <example>ZGlyZWN0aW9uOkRFU0M7bGltaXQ6MTA7cGFnaW5nOkJhY2t3YXJkO3dhbGxldDo7cG9pbnRlcjpOalV6Ow==</example>
        public string Previous { get; set; }
    }
}
