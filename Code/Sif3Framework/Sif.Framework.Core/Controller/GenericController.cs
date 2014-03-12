﻿/*
 * Copyright 2014 Systemic Pty Ltd
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Sif.Framework.Model.Persistence;
using Sif.Framework.Service;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sif.Framework.Controller
{

    public abstract class GenericController<T, PK> : BaseController where T : IPersistable<PK>
    {
        protected IGenericService<T, PK> service;

        // Need to inject service.
        [NonAction]
        protected abstract IGenericService<T, PK> GetService();

        public GenericController()
        {
            service = GetService();
        }

        // DELETE api/{controller}/{id}
        public virtual void Delete(PK id)
        {

            if (!VerifyAuthorisationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            try
            {
                T item = service.Retrieve(id);

                if (item == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                else
                {
                    service.Delete(id);
                }

            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

        }

        // GET api/{controller}/{id}
        public virtual T Get(PK id)
        {

            if (!VerifyAuthorisationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            T item;

            try
            {
                item = service.Retrieve(id);
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return item;
        }

        // GET api/{controller}
        public virtual ICollection<T> Get()
        {

            if (!VerifyAuthorisationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            ICollection<T> items;

            try
            {
                items = service.Retrieve();
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            return items;
        }

        // POST api/{controller}
        public virtual HttpResponseMessage Post(T item)
        {

            if (!VerifyAuthorisationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            HttpResponseMessage responseMessage = null;

            try
            {
                PK id = service.Create(item);
                responseMessage = Request.CreateResponse<T>(HttpStatusCode.Created, item);
                string uri = Url.Link("DefaultApi", new { id = id });
                responseMessage.Headers.Location = new Uri(uri);
            }
            catch (Exception)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return responseMessage;
        }

        // PUT api/{controller}/{id}
        public virtual void Put(PK id, T item)
        {

            if (!VerifyAuthorisationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            try
            {

                if (ModelState.IsValid)
                {
                    T existingItem = service.Retrieve(id);

                    if (existingItem == null)
                    {
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }
                    else
                    {
                        service.Update(item);
                    }

                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

        }

    }

}
