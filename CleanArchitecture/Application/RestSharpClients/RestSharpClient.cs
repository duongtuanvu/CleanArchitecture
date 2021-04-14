using Application.Common;
using Application.Extensions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.RestSharpClients
{
    public interface IRestSharpClient
    {
        Task<IRestResponse> ExcuteApi(RequestParams rq_params);
    }
    public class RestSharpClient : IRestSharpClient
    {
        public async Task<IRestResponse> ExcuteApi(RequestParams rq_params)
        {
            RestClient client = new RestClient(rq_params.BaseUrl);
            RestRequest request = new RestRequest(rq_params.ApiEndPoint,
                rq_params.Method.Equals(Constant.GET) ? Method.GET :
                rq_params.Method.Equals(Constant.POST) ? Method.POST :
                rq_params.Method.Equals(Constant.PUT) ? Method.PUT : Method.DELETE);

            foreach (var header in rq_params.Headers)
            {
                request.AddHeader(header.Key, header.Value);
            }

            if (rq_params.Method.Equals(Constant.GET))
            {
                foreach (var param in rq_params.Parameters)
                {
                    if (rq_params.isUrlSegment)
                    {
                        request.AddParameter(param.Key, param.Value, ParameterType.UrlSegment);
                    }
                    else
                    {
                        request.AddParameter(param.Key, param.Value, ParameterType.GetOrPost);
                    }
                }
            }
            else
            {
                request.AddJsonBody(rq_params.Body);
            }
            //if (rq_params.Method.Equals(Resource.METHOD_POST) || rq_params.Method.Equals(Resource.METHOD_PUT))
            //{

            //}
            //if (rq_params.Method.Equals(Resource.METHOD_DELETE))
            //{
            //    request.AddJsonBody(rq_params.Body);
            //}

            return await client.ExecuteAsync(request);


            #region Example
            //var client = new RestClient("http://example.com");
            //// client.Authenticator = new HttpBasicAuthenticator(username, password);

            //var request = new RestRequest("resource/{id}", Method.POST);
            //request.AddParameter("name", "value"); // adds to POST or URL querystring based on Method
            //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

            //// add parameters for all properties on an object
            //request.AddObject(object);

            //// or just whitelisted properties
            //request.AddObject(object, "PersonId", "Name", ...);

            //// easily add HTTP Headers
            //request.AddHeader("header", "value");

            //// add files to upload (works with compatible verbs)
            //request.AddFile("file", path);

            //// execute the request
            //IRestResponse response = client.Execute(request);
            //var content = response.Content; // raw content as string

            //// or automatically deserialize result
            //// return content type is sniffed but can be explicitly set via RestClient.AddHandler();
            //IRestResponse<Person> response2 = client.Execute<Person>(request);
            //var name = response2.Data.Name;

            //// or download and save file to disk
            //client.DownloadData(request).SaveAs(path);

            //// easy async support
            //await client.ExecuteAsync(request);

            //// async with deserialization
            //var asyncHandle = client.ExecuteAsync<Person>(request, response => {
            //    Console.WriteLine(response.Data.Name);
            //});

            //// abort the request on demand
            //asyncHandle.Abort();
            #endregion
        }
    }

    public class RequestParams
    {
        public RequestParams()
        {
            Headers = new Dictionary<string, string>();
            Parameters = new Dictionary<string, string>();

        }
        public string BaseUrl { get; set; }
        public string ApiEndPoint { get; set; }
        public object Body { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public string Method { get; set; }
        public bool isUrlSegment { get; set; }
    }
}
