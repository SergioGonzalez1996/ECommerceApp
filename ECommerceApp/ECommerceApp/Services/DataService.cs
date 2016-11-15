using ECommerceApp.Classes;
using ECommerceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerceApp.Services
{
    public class DataService
    {
        public User GetUser()
        {
            using (var da = new DataAccess())
            {
                return da.First<User>(false);
            }
        }

        public Response UpdateUser (User user)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    da.Update(user);
                }
                return new Response
                {
                    IsSucces = true,
                    Message = "Usuario actualizado Ok",
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

        public Response InsertUser (User user)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var oldUser = da.First<User>(false);
                    if (oldUser != null)
                    {
                        da.Delete(oldUser);
                    }

                    da.Insert(user);
                }
                return new Response
                {
                    IsSucces = true,
                    Message = "Usuario insertado Ok",
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

        public void Save<T>(List<T> list) where T : class
        {
            using (var da = new DataAccess())
            {
                var oldRecords = da.GetList<T>(false);
                foreach (var record in oldRecords)
                {
                    da.Delete(record);
                }

                foreach (var record in list)
                {
                    da.Insert(record);
                }
            }
        }

        public List<Product> GetProducts(string filter)
        {
            using (var da = new DataAccess())
            {
                return da.GetList<Product>(true)
                    .OrderBy(p => p.Description)
                    .Where(p => p.Description.ToUpper().Contains(filter.ToUpper()))
                    .ToList();
            }
        }

        public List<Customer> GetCustomers(string filter)
        {
            using (var da = new DataAccess())
            {
                return da.GetList<Customer>(true)
                    .OrderBy(c => c.FirstName)
                    .ThenBy(c => c.LastName)
                    .Where(c => c.FirstName.ToUpper().Contains(filter.ToUpper()) || c.LastName.ToUpper().Contains(filter.ToUpper()))
                    .ToList();
            }
        }

        public List<T> Get<T>(bool withChildren) where T : class
        {
            using (var da = new DataAccess()) 
            {
                return da.GetList<T>(withChildren).ToList();
            }
        }

        public Response Login(string email, string password)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var user = da.First<User>(false);
                    if (user == null)
                    {
                        return new Response
                        {
                            IsSucces = false,
                            Message = "No hay conexión a Internet y no hay un usuario previamente registrado.",
                        };
                    }

                    if (user.UserName.ToUpper() == email.ToUpper() && user.Password == password)
                    {
                        return new Response
                        {
                            IsSucces = true,
                            Message = "Login Ok",
                            Result = user,
                        };
                    }
                    return new Response
                    {
                        IsSucces = false,
                        Message = "Usuario o contraseña incorrectos.",
                    };
                }
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
