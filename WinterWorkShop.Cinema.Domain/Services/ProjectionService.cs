using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class ProjectionService : IProjectionService
    {
        private readonly IProjectionsRepository _projectionsRepository;
        private readonly IAuditoriumService _auditoriumService;

        public ProjectionService(IProjectionsRepository projectionsRepository, IAuditoriumService auditoriumService)
        {
            _projectionsRepository = projectionsRepository;
            _auditoriumService = auditoriumService;
        }

        public async Task<IEnumerable<ProjectionDomainModel>> GetAllAsync()
        {
            var data = await _projectionsRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<ProjectionDomainModel> result = new List<ProjectionDomainModel>();
            ProjectionDomainModel model;
            foreach (var item in data)
            {
                model = new ProjectionDomainModel
                {
                    Id = item.Id,
                    MovieId = item.MovieId,
                    AuditoriumId = item.AuditoriumId,
                    ProjectionTime = item.ProjectionTime,
                    MovieTitle = item.Movie.Title,
                    AuditoriumName = item.Auditorium.Name
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<ResponseModel<ProjectionDomainModel>> GetProjectionById(Guid id)
        {
            Projection result = await _projectionsRepository.GetByIdAsync(id);
            if (result == null)
            {
                return new ResponseModel<ProjectionDomainModel>
                {
                    IsSuccessful = false,
                    ErrorMessage = "Projection doesn't exist!"
                };
            }

            ProjectionDomainModel projectionDomainModel = new ProjectionDomainModel
            {
                Id = result.Id,
                AuditoriumId = result.AuditoriumId,
                AuditoriumName = result.Auditorium.Name,
                MovieId = result.MovieId,
                MovieTitle = result.Movie.Title,
                ProjectionTime = result.ProjectionTime
            };
            return new ResponseModel<ProjectionDomainModel>
            {
                DomainModel = projectionDomainModel,
                IsSuccessful = true
            };
        }

        public async Task<CreateProjectionResultModel> CreateProjection(ProjectionDomainModel domainModel)
        {
            int projectionTime = 3;

            var projectionsAtSameTime = _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId)
                .Where(x => x.ProjectionTime < domainModel.ProjectionTime.AddHours(projectionTime) && x.ProjectionTime > domainModel.ProjectionTime.AddHours(-projectionTime))
                .ToList();

            if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0)
            {
                return new CreateProjectionResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTIONS_AT_SAME_TIME
                };
            }

            var newProjection = new Data.Projection
            {
                MovieId = domainModel.MovieId,
                AuditoriumId = domainModel.AuditoriumId,
                ProjectionTime = domainModel.ProjectionTime,
            };

            var insertedProjection = _projectionsRepository.Insert(newProjection);

            if (insertedProjection == null)
            {
                return new CreateProjectionResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_CREATION_ERROR
                };
            }

            _projectionsRepository.Save();
            CreateProjectionResultModel result = new CreateProjectionResultModel
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Projection = new ProjectionDomainModel
                {
                    Id = insertedProjection.Id,
                    AuditoriumId = insertedProjection.AuditoriumId,
                    MovieId = insertedProjection.MovieId,
                    ProjectionTime = insertedProjection.ProjectionTime
                }
            };

            return result;
        }

        public async Task<ResponseModel<ProjectionDomainModel>> Delete(Guid id)
        {
            var projection = await _projectionsRepository.GetByIdAsync(id);

            if (projection == null)
            {
                return new ResponseModel<ProjectionDomainModel>
                {
                    IsSuccessful = false,
                    ErrorMessage = "Projection doesn't exist!"
                };
            }

            ProjectionDomainModel projectionDomainModel = new ProjectionDomainModel();
            projectionDomainModel.AuditoriumName = projection.Auditorium.Name;
            projectionDomainModel.AuditoriumId = projection.AuditoriumId;
            projectionDomainModel.Id = projection.Id;
            projectionDomainModel.MovieId = projection.MovieId;
            projectionDomainModel.MovieTitle = projection.Movie.Title;
            projectionDomainModel.ProjectionTime = projection.ProjectionTime;

            _projectionsRepository.Delete(id);
            _projectionsRepository.Save();
            return new ResponseModel<ProjectionDomainModel>
            {
                IsSuccessful = true,
                DomainModel = projectionDomainModel
            };
        }

        public async Task<ResponseModel<ProjectionDomainModel>> Update(ProjectionDomainModel projectionDomainModel)
        {
            int projectionTime = 3;

            var projectionsAtSameTime = _projectionsRepository.GetByAuditoriumId(projectionDomainModel.AuditoriumId)
                .Where(x => x.ProjectionTime < projectionDomainModel.ProjectionTime.AddHours(projectionTime) && x.ProjectionTime > projectionDomainModel.ProjectionTime.AddHours(-projectionTime))
                .ToList();

            if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0)
            {
                return new ResponseModel<ProjectionDomainModel>()
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTIONS_AT_SAME_TIME,
                    DomainModel = null
                };
            }

            Projection projection = new Projection()
            {
                Id = projectionDomainModel.Id,
                AuditoriumId = projectionDomainModel.AuditoriumId,
                ProjectionTime = projectionDomainModel.ProjectionTime,
                MovieId = projectionDomainModel.MovieId,
            };

            var data = _projectionsRepository.Update(projection);

            if (data == null)
            {
                return new ResponseModel<ProjectionDomainModel>

                {
                    IsSuccessful = false,
                    ErrorMessage = "Failed to update projection!"
                };
            }

            _projectionsRepository.Save();

            ProjectionDomainModel domainModel = new ProjectionDomainModel()
            {
                Id = data.Id,
                MovieId = data.MovieId,
                AuditoriumId = data.AuditoriumId,
                ProjectionTime = data.ProjectionTime
            };

            return new ResponseModel<ProjectionDomainModel>
            {
                IsSuccessful = true,
                DomainModel = domainModel
            };
        }

        public async Task<ResponseModel<IEnumerable<ProjectionDomainModel>>> GetProjectionsFiltered(FilterProjectionsModel filterProjectionsModel)
        {
            var data = await _projectionsRepository.GetAll();
            if (data == null)
            {
                return new ResponseModel<IEnumerable<ProjectionDomainModel>>()
                {
                    DomainModel = null,
                    ErrorMessage = Messages.PROJECTION_GET_ALL_PROJECTIONS_ERROR,
                    IsSuccessful = false
                };
            }

            ResponseModel<IEnumerable<ProjectionDomainModel>> result = new ResponseModel<IEnumerable<ProjectionDomainModel>>()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                DomainModel = data.Select(item => new ProjectionDomainModel()
                {
                    Id = item.Id,
                    MovieId = item.MovieId,
                    AuditoriumId = item.AuditoriumId,
                    ProjectionTime = item.ProjectionTime,
                    MovieTitle = item.Movie.Title,
                    AuditoriumName = item.Auditorium.Name,
                    Movie = item.Movie
                }).ToList()
            };
            if (!filterProjectionsModel.CinemaId.Equals(0))
            {
                var cinemaAuditoriums = await _auditoriumService.GetAuditoriumByCinemaId(filterProjectionsModel.CinemaId);
                var projectionsModels = new List<Projection>();

                foreach (var proj in data)
                {
                    foreach (var auditorium in cinemaAuditoriums)
                    {
                        if (proj.AuditoriumId == auditorium.Id)
                        {
                            projectionsModels.Add(proj);
                            break;
                        }

                    }
                }

                result = new ResponseModel<IEnumerable<ProjectionDomainModel>>()
                {
                    IsSuccessful = true,
                    ErrorMessage = null,
                    DomainModel = projectionsModels.Select(item => new ProjectionDomainModel()
                    {
                        Id = item.Id,
                        MovieId = item.MovieId,
                        AuditoriumId = item.AuditoriumId,
                        ProjectionTime = item.ProjectionTime,
                        MovieTitle = item.Movie.Title,
                        AuditoriumName = item.Auditorium.Name,
                        Movie = item.Movie
                    }).ToList()
                };
            }
            if (!filterProjectionsModel.AuditoriumId.Equals(0))
            {
                var projectionsWithValidAuditoriumId = result.DomainModel.Where(proj => proj.AuditoriumId == filterProjectionsModel.AuditoriumId);
                result = new ResponseModel<IEnumerable<ProjectionDomainModel>>()
                {
                    IsSuccessful = true,
                    ErrorMessage = null,
                    DomainModel = projectionsWithValidAuditoriumId
                };
            }
            if (!filterProjectionsModel.MovieId.Equals(default(Guid)))
            {
                var projectionsWithValidMovieId = result.DomainModel.Where(proj => proj.MovieId == filterProjectionsModel.MovieId);
                result = new ResponseModel<IEnumerable<ProjectionDomainModel>>()
                {
                    IsSuccessful = true,
                    ErrorMessage = null,
                    DomainModel = projectionsWithValidMovieId
                };
            }
            if (!filterProjectionsModel.DateTime.Equals(null))
            {
                var projectionsWithValiDate = result.DomainModel.Where(proj => proj.ProjectionTime.ToString("yyyy-MM-dd") == filterProjectionsModel.DateTime?.ToString("yyyy-MM-dd"));
                result = new ResponseModel<IEnumerable<ProjectionDomainModel>>()
                {
                    IsSuccessful = true,
                    ErrorMessage = null,
                    DomainModel = projectionsWithValiDate
                };
            }
            return result;
        }
    }
}
