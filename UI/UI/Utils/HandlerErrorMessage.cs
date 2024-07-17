using Core.Wrappers;
using Newtonsoft.Json;

namespace UI.Server.Utils
{
    public class HandlerErrorMessage
    {
        public string BuildError(string response, string errorMessage = null) {
            
            try
            {
                if (string.IsNullOrEmpty(response))
                    return errorMessage;

                var reponseMessgaesList = JsonConvert.DeserializeObject<ProblemDetails>(response);
                errorMessage = "<ul>";
                foreach (var error in JsonConvert.DeserializeObject<List<string>>(reponseMessgaesList.Detail))
                {
                    errorMessage = string.Format("{0}</br>{1}", errorMessage, $"<li>{error}</li>");
                }
                string.Format("{0}{1}", errorMessage, $"</ul>");
                return errorMessage;
            }
            catch
            {
                return errorMessage;
            }            
        }

    }
}
