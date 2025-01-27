﻿using Mobile.Code.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mobile.Code.Services.SQLiteLocal
{
    public class ProjectBuildingSqLiteDataStore : IProjectBuilding
    {
        List<ProjectBuilding> items;
        private SQLiteConnection _connection;
        public ProjectBuildingSqLiteDataStore()
        {

            items = new List<ProjectBuilding>();

            _connection = DependencyService.Get<ISQLite>().GetConnection();
            _connection.CreateTable<ProjectBuilding>();

        }

        public async Task<bool> UpdateItemAsync(ProjectBuilding item)
        {
            Response res = new Response();
            try
            {
                _connection.Update(item);
                res.Message = "Record Updated Successfully";
                res.Status = ApiResult.Success;

            }
            catch (Exception)
            {
                res.Message = "Updation Failed";
                res.Status = ApiResult.Fail;
            }

            return await Task.FromResult(true);
        }

        public async Task<Response> DeleteItemAsync(ProjectBuilding item)
        {
            Response res = new Response();
            try
            {
                _connection.Delete<ProjectBuilding>(item.Id);
                foreach (var buildingLoc in _connection.Table<BuildingLocation>().Where(x => x.BuildingId == item.Id))
                {
                    BuildingLocationSqLiteDataStore dq = new BuildingLocationSqLiteDataStore();
                    await dq.DeleteItemAsync(buildingLoc);
                    //_connection.Delete<ProjectBuilding>(buildingLoc.Id);
                }

                res.Message = "Record Deleted Successfully";
                res.Status = ApiResult.Success;

            }
            catch (Exception)
            {
                res.Message = "Deletion Failed";
                res.Status = ApiResult.Fail;
            }

            return await Task.FromResult(res);
        }
        public async Task<Response> AddItemAsync(ProjectBuilding item)
        {

            Response res = new Response(); //not being used now.
            try
            {
                if (item.Id == null)
                {
                    var projectBuilding = new ProjectBuilding
                    {
                        Id = item.Id ?? Guid.NewGuid().ToString(),
                        Name = item.Name,
                        Description = item.Description,
                        ProjectId = item.ProjectId,
                        UserId = App.LogUser.Id.ToString(),
                        ImageDescription = item.ImageDescription,
                        ImageName = item.ImageName,
                        ImageUrl = item.ImageUrl,
                        OnlineId = item.OnlineId
                    };

                    res.TotalCount = _connection.Insert(projectBuilding);


                    res.ID = projectBuilding.Id;
                    res.Data = projectBuilding;
                    res.Message = "Record Inserted Successfully";
                    res.Status = ApiResult.Success;
                }
                else
                {
                    var proj = _connection.Table<ProjectBuilding>().FirstOrDefault(t => t.Id == item.Id);
                    if (proj != null)
                    {
                        int updateStatus = _connection.Update(item);
                        if (updateStatus == 0)
                        {
                            res.Message = "Record Updation failed";
                            res.Data = updateStatus;
                            res.Status = ApiResult.Fail;
                        }
                        else
                        {
                            res.Message = "Record Updated Successfully";
                            res.Data = item;
                            res.Status = ApiResult.Success;
                        }
                    }
                    else
                    {
                        var projectBuilding = new ProjectBuilding
                        {
                            Id = item.Id ?? Guid.NewGuid().ToString(),
                            Name = item.Name,
                            Description = item.Description,
                            ProjectId = item.ProjectId,
                            UserId = App.LogUser.Id.ToString(),
                            ImageDescription = item.ImageDescription,
                            ImageName = item.ImageName,
                            ImageUrl = item.ImageUrl,
                            OnlineId = item.OnlineId
                        };
                        res.TotalCount = _connection.Insert(projectBuilding);
                        res.ID = projectBuilding.Id;
                        res.Data = projectBuilding;
                        res.Message = "Record Inserted Successfully";
                        res.Status = ApiResult.Success;
                    }
                }



            }
            catch (Exception ex)
            {
                res.Message = "Insertion failed." + ex.Message;
                res.Status = ApiResult.Fail;

            }
            return await Task.FromResult(res);

        }


        public async Task<ProjectBuilding> GetItemAsync(string id)
        {
            return await Task.FromResult(_connection.Table<ProjectBuilding>().FirstOrDefault(t => t.Id == id));
        }

        public async Task<IEnumerable<ProjectBuilding>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(from t in _connection.Table<ProjectBuilding>() select t);
        }


        public async Task<IEnumerable<ProjectBuilding>> GetItemsAsyncByProjectID(string projectId)
        {

            return await Task.FromResult(_connection.Table<ProjectBuilding>().Where(t => t.ProjectId == projectId));
        }
    }
}