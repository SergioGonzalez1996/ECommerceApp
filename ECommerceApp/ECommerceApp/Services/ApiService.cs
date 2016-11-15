using ECommerceApp.Classes;
using ECommerceApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ECommerceApp.Services
{
    public class ApiService
    {
        public async Task<Response> Login(string email, string passsword)
        {
            try
            {
                var loginRequest = new LoginRequest
                {
                    Email = email,
                    Password = passsword,
                };

                var request = JsonConvert.SerializeObject(loginRequest);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://zulu-software.com");
                var url = "/ECommerce/api/Users/Login";
                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSucces = false,
                        Message = "Usuario o contraseña incorrectos.",
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(result);

                return new Response
                {
                    IsSucces = true,
                    Message = "Login Ok.",
                    Result = user,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSucces = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response> NewCustomer(Customer customer)
        {
            try
            {
                var request = JsonConvert.SerializeObject(customer);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://zulu-software.com");
                var url = "/ECommerce/api/Customers";
                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSucces = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                var newCustomer = JsonConvert.DeserializeObject<Customer>(result);

                return new Response
                {
                    IsSucces = true,
                    Message = "Cliente creado Ok",
                    Result = newCustomer,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSucces = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<List<T>> Get<T>(string controller) where T : class
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://zulu-software.com");
                var url = string.Format("/ECommerce/api/{0}", controller);
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<T>>(result);
                return list;
            }
            catch
            {
                return null;
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public async Task<Response> SetPhoto(int customerId, Stream stream)
        {
            try
            {
                var array = ReadFully(stream);

                var photoRequest = new PhotoRequest
                {
                    Id = customerId,
                    Array = array,
                };

                var request = JsonConvert.SerializeObject(photoRequest);
                var body = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://zulu-software.com");
                var url = "/ECommerce/api/Customers/SetPhoto";
                var response = await client.PostAsync(url, body);

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSucces = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                return new Response
                {
                    IsSucces = true,
                    Message = "Foto asignada Ok",
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSucces = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response> Update<T>(T model, string controller)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://zulu-software.com");
                var url = string.Format("/ECommerce/api/{0}/{1}", controller, model.GetHashCode());
                var response = await client.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSucces = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                var newRecord = JsonConvert.DeserializeObject<T>(result);

                return new Response
                {
                    IsSucces = true,
                    Message = "Registro Actualizado Ok",
                    Result = newRecord,
                };

            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSucces = false,
                    Message = ex.Message,
                };
            }
        }
    }
}
