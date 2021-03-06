﻿using Entity.Model;
using Entity.Model.BoxResponse;
using Newtonsoft.Json;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebService.Driver
{
    public class BoxService
    {
        private static HttpClient _httpClient;

        /// <summary>
        /// Инициализация экземпляра клиента
        /// </summary>
        /// <param name="client"></param>
        public static void InitializeClient(HttpClient client)
        {
            _httpClient = client;
        }

        /// <summary>
        /// Список всех контейнеров водителя.
        /// </summary>
        /// <returns></returns>
        public static async Task<ServiceResponseObject<ListBoxResponse>> GetContainers()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"containers");

                string s_result;
                using (HttpContent responseContent = response.Content)
                {
                    s_result = await responseContent.ReadAsStringAsync();
                }

                ServiceResponseObject<ListBoxResponse> o_data =
                    new ServiceResponseObject<ListBoxResponse>();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        {
                            ErrorResponseObject error = new ErrorResponseObject();
                            error = JsonConvert.DeserializeObject<ErrorResponseObject>(s_result);
                            o_data.Status = response.StatusCode;
                            o_data.Message = error.Errors[0];
                            return o_data;
                        }
                    case HttpStatusCode.InternalServerError:
                        {
                            throw new Exception("Внутренняя ошибка сервера 500");
                        }

                    case HttpStatusCode.NotFound:
                        {
                            throw new Exception("Ресурс не найден 404");
                        }
                    case HttpStatusCode.OK:
                        {
                            var box = JsonConvert.DeserializeObject<ListBoxResponse>(s_result);
                            o_data.Message = "Успешно!";
                            o_data.Status = response.StatusCode;// а почему переменная container_id пустая
                            o_data.ResponseData = new ListBoxResponse
                            {
                                CONTAINERS = box.CONTAINERS,
                                DEPOT_CONTAINERS = box.DEPOT_CONTAINERS
                            };
                            return o_data;
                        }
                    default:
                        {
                            throw new Exception(response.StatusCode.ToString() + " Server Error");
                        }
                }

               
            }//can not access to close stream 
            catch (Exception ex)
            {
                ServiceResponseObject<ListBoxResponse> o_data = new ServiceResponseObject<ListBoxResponse>();
                o_data.Message = ex.Message;
                return o_data;
            }
        }


        /// <summary>
        /// Получение событий для контейнера.
        /// </summary>
        /// <returns></returns>
        public static async Task<ServiceResponseObject<EventsBoxResponse>> Events(string event_id)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"container/{event_id}/events");

                string s_result;
                using (HttpContent responseContent = response.Content)
                {
                    s_result = await responseContent.ReadAsStringAsync();
                }

                ServiceResponseObject<EventsBoxResponse> o_data =
                    new ServiceResponseObject<EventsBoxResponse>();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        {
                            throw new Exception("Событий этого контейнера нет");
                        }
                    case HttpStatusCode.InternalServerError:
                        {
                            throw new Exception("Внутренняя ошибка сервера 500");
                        }

                    case HttpStatusCode.NotFound:
                        {
                            throw new Exception("Ресурс не найден 404");
                        }
                    case HttpStatusCode.OK:
                        {
                            var box = JsonConvert.DeserializeObject<EventsBoxResponse>(s_result);
                            o_data.Message = "Успешно!";
                            o_data.Status = response.StatusCode;// а почему переменная container_id пустая
                            o_data.ResponseData = new EventsBoxResponse
                            {
                                CONTAINER = box.CONTAINER,
                                EVENTS = box.EVENTS
                            };
                            return o_data;
                        }
                    default:
                        {
                            throw new Exception(response.StatusCode.ToString() + " Server Error");
                        }
                }


            }//can not access to close stream 
            catch (Exception ex)
            {
                ServiceResponseObject<EventsBoxResponse> o_data = new ServiceResponseObject<EventsBoxResponse>();
                o_data.Message = ex.Message;
                return o_data;
            }
        }

        /// <summary>
        /// Получение данных контейнера.
        /// </summary>
        /// <returns></returns>
        public static async Task<ServiceResponseObject<InfoBoxResponse>> GetInfoBox(string CONTAINER_ID)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://smartboxcity.ru:8003/container/" + CONTAINER_ID);
                request.Method = "GET";
                request.Credentials = new NetworkCredential(CrossSettings.Current.GetValueOrDefault("token", ""), "");

                var response = (HttpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();



                StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

                string s_result = myStreamReader.ReadToEnd();

                myStreamReader.Close();
                responseStream.Close();

                response.Close();

                //HttpResponseMessage response = await _httpClient.GetAsync($"container/{CONTAINER_ID}");

                //string s_result;
                //using (HttpContent responseContent = response.Content)
                //{
                //    s_result = await responseContent.ReadAsStringAsync();
                //}

                ServiceResponseObject<InfoBoxResponse> o_data =
                    new ServiceResponseObject<InfoBoxResponse>();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        {
                            ErrorResponseObject error = new ErrorResponseObject();
                            error = JsonConvert.DeserializeObject<ErrorResponseObject>(s_result);
                            o_data.Status = response.StatusCode;
                            o_data.Message = "Ошибка 400";
                            return o_data;
                        }
                    case HttpStatusCode.InternalServerError:
                        {
                            throw new Exception("Внутренняя ошибка сервера 500");
                        }

                    case HttpStatusCode.NotFound:
                        {
                            throw new Exception("Ресурс не найден 404");
                        }
                    case HttpStatusCode.OK:
                        {
                            var box = JsonConvert.DeserializeObject<InfoBoxResponse>(s_result);
                            o_data.Message = "Успешно!";
                            o_data.Status = response.StatusCode;// а почему переменная container_id пустая
                            o_data.ResponseData = new InfoBoxResponse
                            {
                                CONTAINER = box.CONTAINER,
                                SENSORS_STATUS = box.SENSORS_STATUS,
                                ALARMS_STATUS = box.ALARMS_STATUS,
                                EVENT_COUNT = box.EVENT_COUNT
                            };
                            return o_data;
                        }
                    default:
                        {
                            throw new Exception(response.StatusCode.ToString() + " Server Error");
                        }
                }

                
            }//can not access to close stream 
            catch (Exception ex)
            {
                ServiceResponseObject<InfoBoxResponse> o_data = new ServiceResponseObject<InfoBoxResponse>();
                o_data.Message = ex.Message;
                return o_data;
            }
        }

        /// <summary>
        /// Разложить контейнер.
        /// </summary>
        /// <returns></returns>
        public static async Task<ServiceResponseObject<SuccessResponse>> UnfoldContainer(string CONTAINER_ID)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"container/{CONTAINER_ID}/unfold");

                string s_result;
                using (HttpContent responseContent = response.Content)
                {
                    s_result = await responseContent.ReadAsStringAsync();
                }

                ServiceResponseObject<SuccessResponse> o_data =
                    new ServiceResponseObject<SuccessResponse>();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        {
                            ErrorResponseObject error = new ErrorResponseObject();
                            error = JsonConvert.DeserializeObject<ErrorResponseObject>(s_result);
                            o_data.Status = response.StatusCode;
                            o_data.Message = error.Errors[0];
                            return o_data;
                        }
                    case HttpStatusCode.InternalServerError:
                        {
                            throw new Exception("Внутренняя ошибка сервера 500");
                        }

                    case HttpStatusCode.NotFound:
                        {
                            throw new Exception("Ресурс не найден 404");
                        }
                    case HttpStatusCode.OK:
                        {
                            var box = JsonConvert.DeserializeObject<SuccessResponse>(s_result);
                            o_data.Message = "Успешно!";
                            o_data.Status = response.StatusCode;// а почему переменная container_id пустая
                            o_data.ResponseData = new SuccessResponse
                            {
                                Message = box.Message
                            };
                            return o_data;
                        }
                    default:
                        {
                            throw new Exception(response.StatusCode.ToString() + " Server Error");
                        }
                }

               
            }//can not access to close stream 
            catch (Exception ex)
            {
                ServiceResponseObject<SuccessResponse> o_data = new ServiceResponseObject<SuccessResponse>();
                o_data.Message = ex.Message;
                return o_data;
            }
        }


        /// <summary>
        /// Сложить контейнер.
        /// </summary>
        /// <returns></returns>
        public static async Task<ServiceResponseObject<SuccessResponse>> FoldContainer(string CONTAINER_ID)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"container/{CONTAINER_ID}/fold");

                string s_result;
                using (HttpContent responseContent = response.Content)
                {
                    s_result = await responseContent.ReadAsStringAsync();
                }

                ServiceResponseObject<SuccessResponse> o_data =
                    new ServiceResponseObject<SuccessResponse>();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        {
                            ErrorResponseObject error = new ErrorResponseObject();
                            error = JsonConvert.DeserializeObject<ErrorResponseObject>(s_result);
                            o_data.Status = response.StatusCode;
                            o_data.Message = error.Errors[0];
                            return o_data;
                        }
                    case HttpStatusCode.InternalServerError:
                        {
                            throw new Exception("Внутренняя ошибка сервера 500");
                        }

                    case HttpStatusCode.NotFound:
                        {
                            throw new Exception("Ресурс не найден 404");
                        }
                    case HttpStatusCode.OK:
                        {
                            var box = JsonConvert.DeserializeObject<SuccessResponse>(s_result);
                            o_data.Message = "Успешно!";
                            o_data.Status = response.StatusCode;// а почему переменная container_id пустая
                            o_data.ResponseData = new SuccessResponse
                            {
                                Message = box.Message
                            };
                            return o_data;
                        }
                    default:
                        {
                            throw new Exception(response.StatusCode.ToString() + " Server Error");
                        }
                }

               
            }//can not access to close stream 
            catch (Exception ex)
            {
                ServiceResponseObject<SuccessResponse> o_data = new ServiceResponseObject<SuccessResponse>();
                o_data.Message = ex.Message;
                return o_data;
            }
        }

        /// <summary>
        /// Передать контейнер другому водителю.
        /// </summary>
        /// <returns></returns>
        public static async Task<ServiceResponseObject<SuccessResponse>> Dettach(string CONTAINER_ID, bool isChecked)
        {
            try
            {
                HttpResponseMessage response = (isChecked)?await _httpClient.GetAsync($"container/{CONTAINER_ID}/detach?task=1"):
                    await _httpClient.GetAsync($"container/{CONTAINER_ID}/detach");

                string s_result;
                using (HttpContent responseContent = response.Content)
                {
                    s_result = await responseContent.ReadAsStringAsync();
                }

                ServiceResponseObject<SuccessResponse> o_data =
                    new ServiceResponseObject<SuccessResponse>();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        {
                            throw new Exception("Ошибка сервера 400");
                        }
                    case HttpStatusCode.InternalServerError:
                        {
                            throw new Exception("Внутренняя ошибка сервера 500");
                        }

                    case HttpStatusCode.NotFound:
                        {
                            throw new Exception("Ресурс не найден 404");
                        }
                    case HttpStatusCode.OK:
                        {
                            var box = JsonConvert.DeserializeObject<SuccessResponse>(s_result);
                            o_data.Message = "Успешно!";
                            o_data.Status = response.StatusCode;// а почему переменная container_id пустая
                            o_data.ResponseData = new SuccessResponse
                            {
                                Message = box.Message
                            };
                            return o_data;
                        }
                    default:
                        {
                            throw new Exception(response.StatusCode.ToString() + " Server Error");
                        }
                }

               
            }//can not access to close stream 
            catch (Exception ex)
            {
                ServiceResponseObject<SuccessResponse> o_data = new ServiceResponseObject<SuccessResponse>();
                o_data.Message = ex.Message;
                return o_data;
            }
        }


        /// <summary>
        /// Прикрепить контейнер.
        /// </summary>
        /// <param name="CONTAINER_ID"></param>
        /// <returns></returns>
        public async static Task<ServiceResponseObject<SuccessResponse>> Attach(string CONTAINER_ID)
        {

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"container/{CONTAINER_ID}/attach");

                string s_result;
                using (HttpContent responseContent = response.Content)
                {
                    s_result = await responseContent.ReadAsStringAsync();
                }

                ServiceResponseObject<SuccessResponse> o_data =
                    new ServiceResponseObject<SuccessResponse>();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        {
                            ErrorResponseObject error = new ErrorResponseObject();
                            error = JsonConvert.DeserializeObject<ErrorResponseObject>(s_result);
                            o_data.Status = response.StatusCode;
                            o_data.Message = error.Errors[0];
                            return o_data;
                        }
                    case HttpStatusCode.InternalServerError:
                        {
                            throw new Exception("Внутренняя ошибка сервера 500");
                        }

                    case HttpStatusCode.NotFound:
                        {
                            throw new Exception("Ресурс не найден 404");
                        }
                    case HttpStatusCode.OK:
                        {
                            var box = JsonConvert.DeserializeObject<SuccessResponse>(s_result);
                            o_data.Message = "Успешно!";
                            o_data.Status = response.StatusCode;// а почему переменная container_id пустая
                            o_data.ResponseData = new SuccessResponse
                            {
                                Message = box.Message
                            };
                            return o_data;
                        }
                    default:
                        {
                            throw new Exception(response.StatusCode.ToString() + " Server Error");
                        }
                }

               
            }//can not access to close stream 
            catch (Exception ex)
            {
                ServiceResponseObject<SuccessResponse> o_data = new ServiceResponseObject<SuccessResponse>();
                o_data.Message = ex.Message;
                return o_data;
            }
        }

        /// <summary>
        /// Получить фотофиксацию с контейнера.
        /// </summary>
        /// <returns></returns>
        public static async Task<ServiceResponseObject<SuccessResponse>> GetPhoto(string CONTAINER_ID)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"container/{CONTAINER_ID}/image");

                string s_result;
                using (HttpContent responseContent = response.Content)
                {
                    s_result = await responseContent.ReadAsStringAsync();
                }

                //HttpWebRequest request =  (HttpWebRequest)WebRequest.Create($"container/{CONTAINER_ID}/image");
                //request.Method = "GET";

                //var myHttpWebResponse = (HttpWebResponse)request.GetResponse();

                //Stream responseStream = myHttpWebResponse.GetResponseStream();

                //StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

                //string s_result = myStreamReader.ReadToEnd();

                //myStreamReader.Close();
                //responseStream.Close();

                //myHttpWebResponse.Close();

                ServiceResponseObject<SuccessResponse> o_data =
                    new ServiceResponseObject<SuccessResponse>();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        {
                            ErrorResponseObject error = new ErrorResponseObject();
                            error = JsonConvert.DeserializeObject<ErrorResponseObject>(s_result);
                            o_data.Status = response.StatusCode;
                            o_data.Message = error.Errors[0];
                            return o_data;
                        }
                    case HttpStatusCode.InternalServerError:
                        {
                            throw new Exception("Внутренняя ошибка сервера 500");
                        }

                    case HttpStatusCode.NotFound:
                        {
                            throw new Exception("Ресурс не найден 404");
                        }
                    case HttpStatusCode.OK:
                        {
                            var box = JsonConvert.DeserializeObject<SuccessResponse>(s_result);
                            o_data.Message = "Успешно!";
                            o_data.Status = response.StatusCode;// а почему переменная container_id пустая
                            o_data.ResponseData = new SuccessResponse
                            {
                                Message = box.Message
                            };
                            return o_data;
                        }
                    default:
                        {
                            throw new Exception(response.StatusCode.ToString() + " Server Error");
                        }
                }
            }//can not access to close stream 
            catch (Exception ex)
            {
                ServiceResponseObject<SuccessResponse> o_data = new ServiceResponseObject<SuccessResponse>();
                o_data.Message = ex.Message;
                return o_data;
            }
        }

        /// <summary>
        /// Получить видео с контейнера.
        /// </summary>
        /// <returns></returns>
        public static async Task<ServiceResponseObject<SuccessResponse>> GetVideo(string CONTAINER_ID)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"container/{CONTAINER_ID}/video");

                string s_result;
                using (HttpContent responseContent = response.Content)
                {
                    s_result = await responseContent.ReadAsStringAsync();
                }

                ServiceResponseObject<SuccessResponse> o_data =
                    new ServiceResponseObject<SuccessResponse>();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        {
                            ErrorResponseObject error = new ErrorResponseObject();
                            error = JsonConvert.DeserializeObject<ErrorResponseObject>(s_result);
                            o_data.Status = response.StatusCode;
                            o_data.Message = error.Errors[0];
                            return o_data;
                        }
                    case HttpStatusCode.InternalServerError:
                        {
                            throw new Exception("Внутренняя ошибка сервера 500");
                        }

                    case HttpStatusCode.NotFound:
                        {
                            throw new Exception("Ресурс не найден 404");
                        }
                    case HttpStatusCode.OK:
                        {
                            var box = JsonConvert.DeserializeObject<SuccessResponse>(s_result);
                            o_data.Message = "Успешно!";
                            o_data.Status = response.StatusCode;
                            o_data.ResponseData = new SuccessResponse
                            {
                                Message = box.Message
                            };
                            return o_data;
                        }
                    default:
                        {
                            throw new Exception(response.StatusCode.ToString() + " Server Error");
                        }
                }
            }//can not access to close stream 
            catch (Exception ex)
            {
                ServiceResponseObject<SuccessResponse> o_data = new ServiceResponseObject<SuccessResponse>();
                o_data.Message = ex.Message;
                return o_data;
            }
        }

        public static async Task<ServiceResponseObject<SuccessResponse>> CheckFile(string file_name)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("https://smartboxcity.ru/media_json.php?media=" + file_name);
                string s_result;
                using (HttpContent responseContent = response.Content)
                {
                    s_result = await responseContent.ReadAsStringAsync();
                }

                ServiceResponseObject<SuccessResponse> o_data = new ServiceResponseObject<SuccessResponse>();
                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        {
                            throw new Exception("Ошибка сервера 400");
                        }
                    case HttpStatusCode.InternalServerError:
                        {
                            throw new Exception("Внутренняя ошибка сервера 500");
                        }

                    case HttpStatusCode.NotFound:
                        {
                            throw new Exception("Ресурс не найден 404");
                        }
                    case HttpStatusCode.OK:
                        {
                            var message = JsonConvert.DeserializeObject<SuccessResponse>(s_result);
                            o_data.Message = message.Message;
                            o_data.Status = response.StatusCode;
                            return o_data;
                        }
                    default:
                        {
                            throw new Exception(response.StatusCode.ToString() + " Server Error");
                        }
                }
            }
            catch (Exception ex)
            {
                ServiceResponseObject<SuccessResponse> o_data = new ServiceResponseObject<SuccessResponse>();
                o_data.Message = ex.Message;
                return o_data;
            }
        }

        /// <summary>
        /// Поднять роллету.
        /// </summary>
        /// <returns></returns>
        public static async Task<ServiceResponseObject<SuccessResponse>> UnlockRollete(string CONTAINER_ID)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"container/{CONTAINER_ID}/unlock");

                string s_result;
                using (HttpContent responseContent = response.Content)
                {
                    s_result = await responseContent.ReadAsStringAsync();
                }

                ServiceResponseObject<SuccessResponse> o_data =
                    new ServiceResponseObject<SuccessResponse>();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        {
                            ErrorResponseObject error = new ErrorResponseObject();
                            error = JsonConvert.DeserializeObject<ErrorResponseObject>(s_result);
                            o_data.Status = response.StatusCode;
                            o_data.Message = error.Errors[0];
                            return o_data;
                        }
                    case HttpStatusCode.InternalServerError:
                        {
                            throw new Exception("Внутренняя ошибка сервера 500");
                        }

                    case HttpStatusCode.NotFound:
                        {
                            throw new Exception("Ресурс не найден 404");
                        }
                    case HttpStatusCode.OK:
                        {
                            var box = JsonConvert.DeserializeObject<SuccessResponse>(s_result);
                            o_data.Message = "Успешно!";
                            o_data.Status = response.StatusCode;// а почему переменная container_id пустая
                            o_data.ResponseData = new SuccessResponse
                            {
                                Message = box.Message
                            };
                            return o_data;
                        }
                    default:
                        {
                            throw new Exception(response.StatusCode.ToString() + " Server Error");
                        }
                }
            }//can not access to close stream 
            catch (Exception ex)
            {
                ServiceResponseObject<SuccessResponse> o_data = new ServiceResponseObject<SuccessResponse>();
                o_data.Message = ex.Message;
                return o_data;
            }
        }


        /// <summary>
        /// Опустить роллету.
        /// </summary>
        /// <returns></returns>
        public static async Task<ServiceResponseObject<SuccessResponse>> LockRollete(string CONTAINER_ID)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"container/{CONTAINER_ID}/lock");

                string s_result;
                using (HttpContent responseContent = response.Content)
                {
                    s_result = await responseContent.ReadAsStringAsync();
                }

                ServiceResponseObject<SuccessResponse> o_data =
                    new ServiceResponseObject<SuccessResponse>();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        {
                            ErrorResponseObject error = new ErrorResponseObject();
                            error = JsonConvert.DeserializeObject<ErrorResponseObject>(s_result);
                            o_data.Status = response.StatusCode;
                            o_data.Message = error.Errors[0];
                            return o_data;
                        }
                    case HttpStatusCode.InternalServerError:
                        {
                            throw new Exception("Внутренняя ошибка сервера 500");
                        }

                    case HttpStatusCode.NotFound:
                        {
                            throw new Exception("Ресурс не найден 404");
                        }
                    case HttpStatusCode.OK:
                        {
                            var box = JsonConvert.DeserializeObject<SuccessResponse>(s_result);
                            o_data.Message = "Успешно!";
                            o_data.Status = response.StatusCode;// а почему переменная container_id пустая
                            o_data.ResponseData = new SuccessResponse
                            {
                                Message = box.Message
                            };
                            return o_data;
                        }
                    default:
                        {
                            throw new Exception(response.StatusCode.ToString() + " Server Error");
                        }
                }
            }//can not access to close stream 
            catch (Exception ex)
            {
                ServiceResponseObject<SuccessResponse> o_data = new ServiceResponseObject<SuccessResponse>();
                o_data.Message = ex.Message;
                return o_data;
            }
        }


        /// <summary>
        /// Остановить исполнение команды.
        /// </summary>
        /// <returns></returns>
        public static async Task<ServiceResponseObject<SuccessResponse>> StopCommands(string CONTAINER_ID)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"container/{CONTAINER_ID}/stop");

                string s_result;
                using (HttpContent responseContent = response.Content)
                {
                    s_result = await responseContent.ReadAsStringAsync();
                }

                ServiceResponseObject<SuccessResponse> o_data =
                    new ServiceResponseObject<SuccessResponse>();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        {
                            ErrorResponseObject error = new ErrorResponseObject();
                            error = JsonConvert.DeserializeObject<ErrorResponseObject>(s_result);
                            o_data.Status = response.StatusCode;
                            o_data.Message = error.Errors[0];
                            return o_data;
                        }
                    case HttpStatusCode.InternalServerError:
                        {
                            throw new Exception("Внутренняя ошибка сервера 500");
                        }

                    case HttpStatusCode.NotFound:
                        {
                            throw new Exception("Ресурс не найден 404");
                        }
                    case HttpStatusCode.OK:
                        {
                            var box = JsonConvert.DeserializeObject<SuccessResponse>(s_result);
                            o_data.Message = "Успешно!";
                            o_data.Status = response.StatusCode;// а почему переменная container_id пустая
                            o_data.ResponseData = new SuccessResponse
                            {
                                Message = box.Message
                            };
                            return o_data;
                        }
                    default:
                        {
                            throw new Exception(response.StatusCode.ToString() + " Server Error");
                        }
                }
            }//can not access to close stream 
            catch (Exception ex)
            {
                ServiceResponseObject<SuccessResponse> o_data = new ServiceResponseObject<SuccessResponse>();
                o_data.Message = ex.Message;
                return o_data;
            }
        }
    }
}
