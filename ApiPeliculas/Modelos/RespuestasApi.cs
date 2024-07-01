using System.Net;

namespace ApiPeliculas.Modelos {
    public class RespuestasApi {
         public RespuestasApi() {
            ErrorMessages = new List<string>();
        }

        public HttpStatusCode StatusCode { get; set; }
        public List<string> ErrorMessages { get; set; }
        public bool IsSuccess { get; set; }
        public object Result { get; set; }  
    }
}
