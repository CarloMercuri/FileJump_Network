using FileJump_Network;
using FileJump_Network.EventSystem;
using FileJump_Network.Interfaces;
using FileJump_Network.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public static class ApiCommunication
    {
        public static HttpClient ApiClient { get; private set; }

        private static int progressCount = 0;

        // EVENTS

        public static event EventHandler<LoginResultEventArgs> LoginActionResult;
        public static event EventHandler<LogoutResultEventArgs> LogoutActionResult;

        public static event EventHandler<RegistrationResultEventArgs> RegistrationActionResult;

        public static event EventHandler<ThumbnailReceivedEventArgs> ThumbnailReceivedResult;

        public static event EventHandler<EventArgs> UserAuthenticationInvalid;

        public static event EventHandler<FileDownloadResultEventArgs> FileDownloadSuccessful;
        public static event EventHandler<FileDownloadResultEventArgs> FileDownloadFailure;

        public static event EventHandler<FileTransferProgressEventArgs> FileDownloadProgress;
        public static event EventHandler<FileTransferProgressEventArgs> FileUploadProgress;
        private static EventHandler<FileDeleteRequestEventArgs> _FileDeletedResponse;

        public static event EventHandler<FileDeleteRequestEventArgs> FileDeletedResponse
        {
            add
            {
                if (_FileDeletedResponse == null || !_FileDeletedResponse.GetInvocationList().Contains(value))
                {
                    _FileDeletedResponse += value;
                }
            }
            remove
            {
                _FileDeletedResponse -= value;
            }
        }



        private static int downloadProgressTreshold = 5;
        private static int downloadProgressNextTick = 0;

        /// <summary>
        /// Initializes the HttpClient
        /// </summary>
        public static void InitializeClient()
        {
            ApiClient = new HttpClient();
            // Using the connection string found in Constants
            ApiClient.BaseAddress = new Uri(Constants.BaseURL);
            // 8 seconds timeout
            ApiClient.Timeout = TimeSpan.FromSeconds(50);
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            //ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private static string CreateNewAccountJSonData(string _email, string _password, string _firstName, string _lastName)
        {
            JObject jsonObject = new JObject
            {
                { "Email", _email},
                { "Password", _password },
                { "FirstName", _firstName},
                { "LastName", _lastName },
            };

            return jsonObject.ToString();
        }

        private async static Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
        {
            try
            {
                HttpResponseMessage response = await ApiClient.SendAsync(request);
                return response;
            }
            catch (Exception e)
            {
                var response = new HttpResponseMessage();
                response.StatusCode = System.Net.HttpStatusCode.RequestTimeout;
                return response;
            }
        }

        public async static Task<bool> DeleteFileRequest(string fileName, string authToken, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Constants.API_FILEHOST_DELETE_FILE);

            request.Headers.Add("File-Name", fileName);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            HttpResponseMessage response = await SendRequest(request);

            if(response.StatusCode == HttpStatusCode.OK)
            {
                FileDeleteRequestEventArgs args = new FileDeleteRequestEventArgs();
                args.Successful = true;
                args.FileName = fileName;
                args.ID = id;
                _FileDeletedResponse?.Invoke(null, args);
                return true;
            }
            else
            {
                FileDeleteRequestEventArgs args = new FileDeleteRequestEventArgs();
                args.Successful = false;
                args.FileName = fileName;
                args.ID = id;
                _FileDeletedResponse?.Invoke(null, args);
                return false;
            }

        }

        public async static Task<bool> DownloadFile(string fileName, int id, string authToken)
        {
            string tempfile = Path.Combine(Constants.API_FILE_DOWNLOAD_FOLDER, fileName);

            var request = new HttpRequestMessage(HttpMethod.Get, Constants.API_FILEHOST_GET_FILE);

            request.Headers.Add("File-Name", fileName);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);


            HttpResponseMessage response = await ApiClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);


            if(response.StatusCode != HttpStatusCode.OK)
            {
                // Something wrong
                FileDownloadResultEventArgs result_args = new FileDownloadResultEventArgs();
                result_args.Successful = false;
                result_args.FileName = fileName;
                result_args.Reason = response.ReasonPhrase;
                FileDownloadFailure?.Invoke(null, result_args);
                return false;
            }

            var totalBytes = 0L;
                
            if (response.Headers.Contains("File-Size"))
            {
                totalBytes = Convert.ToInt64(response.Headers.GetValues("File-Size").FirstOrDefault());
            }

            Stream contentStream = await response.Content.ReadAsStreamAsync();
            FileStream fileStream = new FileStream(tempfile, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);


            var totalRead = 0L;
            var totalReads = 0L;
            var buffer = new byte[8192];
            var isMoreToRead = true;

            do
            {
                var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                if (read == 0)
                {
                    isMoreToRead = false;
                }
                else
                {
                    await fileStream.WriteAsync(buffer, 0, read);

                    totalRead += read;
                    totalReads += 1;

                    if (totalReads % 2000 == 0)
                    {
                        FileTransferProgressEventArgs progress_args = new FileTransferProgressEventArgs();
                        progress_args.FileName = fileName;
                        progress_args.ID = id;
                        decimal pp = (decimal)totalRead / (decimal)totalBytes;

                        progress_args.PercentProgress = (int)Math.Round(pp * 100);

                        FileDownloadProgress?.Invoke(null, progress_args);


                        //Console.WriteLine(string.Format("total bytes downloaded so far: {0:n0}", totalRead));
                    }
                }
            }
            while (isMoreToRead);



            FileDownloadResultEventArgs args = new FileDownloadResultEventArgs();
            args.Successful = true;
            args.FileName = fileName;
            args.ID = id;

            FileDownloadSuccessful?.Invoke(null, args);

            return true;
        }




        public async static void SendNewAccountRegistrationData(string _email, string _password, string _firstName, string _lastName)
        {

            NewAccountDataModel model = new NewAccountDataModel()
            {
                Email = _email,
                Password = _password,
                FirstName = _firstName,
                LastName = _lastName
            };

            string json = JsonConvert.SerializeObject(model);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, Constants.API_NEW_ACCOUNT_POST);
            requestMessage.Content = new StringContent(json);
            HttpResponseMessage response = await SendRequest(requestMessage);

            RegistrationResultEventArgs args = new RegistrationResultEventArgs();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                args.Successful = true;
                args.Message = response.ReasonPhrase;
            }
            else
            {
                args.Successful = false;
                args.Message = response.ReasonPhrase;
            }

            RegistrationActionResult?.Invoke(null, args);
        }

       


        public async static Task<byte[]> GetThumbnail(string thumb_name, string authToken)
        {

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, Constants.API_FILEHOST_GET_THUMBNAIL);
            requestMessage.Headers.Add("File-Name", thumb_name);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            HttpResponseMessage response = await SendRequest(requestMessage);


            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var array = response.Content.ReadAsByteArrayAsync().Result;
                ThumbnailReceivedEventArgs args = new ThumbnailReceivedEventArgs();
                args.ImageBytes = array;
                return array;
            }
            else
            {
                return null;
            }
            
          
        }

        public async static Task<List<OnlineFileStructure>> RequestHostedFilesList(string authToken)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, Constants.API_FILEHOST_GET_FILES_LIST);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            HttpResponseMessage response = await SendRequest(requestMessage);


            if (response.StatusCode != HttpStatusCode.OK)
            {
                if(response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    UserAuthenticationInvalid?.Invoke(null, new EventArgs());
                }

                return null;
            }

            string responseBody = response.Content.ReadAsStringAsync().Result;
            List<OnlineFileStructure> files_list = JsonConvert.DeserializeObject<List<OnlineFileStructure>>(responseBody);

            return files_list;
        }

       

        public static void SendFileAsync(string filePath, string authToken, int id)
        {
            progressCount = 0;

            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + authToken;
                client.Headers.Add("File-Name", Path.GetFileName(filePath));
                client.BaseAddress = "https://localhost:44388";
 
                client.UploadFileAsync(new Uri("https://localhost:44388/FileHosting/UploadFile"), filePath);
                //client.UploadDataAsync(new Uri("https://localhost:44388/FileHosting/UploadFile"))

                client.UploadProgressChanged += (sender, args) =>
                {
                    UploadProgress(args, filePath, id);
                };

                client.UploadFileCompleted += UploadFilecomplete;
                //string response = Encoding.UTF8.GetString(responseBinary);

                Console.WriteLine();
            }

           
        }

        private static void UploadFilecomplete(object sender, UploadFileCompletedEventArgs e)
        {
            downloadProgressNextTick = 0;

            if(e.Error == null)
            {
                // success
            } 
            else
            {
                Console.WriteLine("Error!: " + e.Error);
            }

           
           
        }

        private static void UploadProgress(UploadProgressChangedEventArgs e, string fileName, int id)
        {
            decimal pp = (decimal)e.BytesSent / (decimal)e.TotalBytesToSend;
            int percent = (int)Math.Round(pp * 100);

            if(percent > downloadProgressNextTick)
            {
                FileTransferProgressEventArgs args = new FileTransferProgressEventArgs();
                args.PercentProgress = percent;
                args.ID = id;
                FileUploadProgress?.Invoke(null, args);
                downloadProgressNextTick = percent + downloadProgressTreshold;
            }
        }

        private async static Task<System.IO.Stream> UploadAsync(string url, string filename, Stream fileStream, byte[] fileBytes)
        {
            // Convert each of the three inputs into HttpContent objects

            HttpContent stringContent = new StringContent(filename);
            // examples of converting both Stream and byte [] to HttpContent objects
            // representing input type file
            HttpContent fileStreamContent = new StreamContent(fileStream);
            HttpContent bytesContent = new ByteArrayContent(fileBytes);

            // Submit the form using HttpClient and 
            // create form data as Multipart (enctype="multipart/form-data")

            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                // Add the HttpContent objects to the form data

                // <input type="text" name="filename" />
                formData.Add(stringContent, "filename", "filename");
                // <input type="file" name="file1" />
                formData.Add(fileStreamContent, "file1", "file1");
                // <input type="file" name="file2" />
                formData.Add(bytesContent, "file2", "file2");

                // Invoke the request to the server

                // equivalent to pressing the submit button on
                // a form with attributes (action="{url}" method="post")
                var response = await client.PostAsync(url, formData);

                // ensure the request was a success
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                return await response.Content.ReadAsStreamAsync();
            }
        }

      

      

        public async static Task<IActionApiResponse> SendPasswordRecoveryRequest(string email)
        {
            dynamic model = new ExpandoObject();
            model.user_email = email;

            string json = JsonConvert.SerializeObject(model);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, Constants.API_PASSWORD_RECOVERY_POST);

            requestMessage.Content = new StringContent(json);
            HttpResponseMessage response = await SendRequest(requestMessage);


            PasswordRecoveryResponseModel recoveryResponse = new PasswordRecoveryResponseModel();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                recoveryResponse.Successful = true;
                recoveryResponse.Response = response;
                return recoveryResponse;
            }
            else
            {
                recoveryResponse.Successful = false;
                recoveryResponse.Response = response;
                return recoveryResponse;
            }
        }

        public async static Task<IActionApiResponse> SendChangePasswordRequest(string old_password, string new_password, string authToken)
        {
            dynamic model = new ExpandoObject();
            model.OldPassword = old_password;
            model.NewPassword = new_password;

            // Convert it
            string json = JsonConvert.SerializeObject(model);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, Constants.API_CHANGE_PASSWORD_POST);

            // Needs to contain the bearer token
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            requestMessage.Content = new StringContent(json);



            HttpResponseMessage response = await SendRequest(requestMessage);

            PasswordChangeResponseModel recoveryResponse = new PasswordChangeResponseModel();
            recoveryResponse.Response = response;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                recoveryResponse.Successful = true;
                return recoveryResponse;
            }
            else
            {
                recoveryResponse.Successful = false;
                return recoveryResponse;
            }
        }
       
        public static void Logout()
        {
            LogoutResultEventArgs args = new LogoutResultEventArgs();
            args.Successful = true;
            LogoutActionResult?.Invoke(null, args);
        }

        
        public async static void SendLoginRequest(string email, string password)
        {

            var authenticationString = $"{email}:{password}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, Constants.API_LOGIN_POST);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

            LoginRequestResponseModel loginResult = new LoginRequestResponseModel();
            HttpResponseMessage response = await SendRequest(requestMessage);

            LoginResultEventArgs args = new LoginResultEventArgs();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string responseBody = response.Content.ReadAsStringAsync().Result;
                AccountInfoModel model = JsonConvert.DeserializeObject<AccountInfoModel>(responseBody);

                args.AccountData = model;
                args.Successful = true;
            }
            else
            {
                args.Message = response.ReasonPhrase;
                args.Successful = false;
                args.AccountData = null;
            }

            LoginActionResult?.Invoke(null, args);
        }


    }
}
