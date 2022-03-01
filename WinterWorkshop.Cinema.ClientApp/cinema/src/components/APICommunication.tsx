import { serviceConfig } from "../appSettings";

import { IAuditorium, IProjection, ICinema, IMovie, IProjectionFilter } from "../models";

export function getAllCinemas(){
    const requestOptions = {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("jwt")}`,
        },
    };
    return fetch(`${serviceConfig.baseURL}/api/cinemas`, requestOptions)
    .then((response) => response.json())
    .then((responseData) => {
      return responseData;
    })
    .catch(error => console.warn(error));
  }
  
export function getAllAuditoriums(){
    const requestOptions = {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("jwt")}`,
        },
      };
      return fetch(`${serviceConfig.baseURL}/api/auditoriums`, requestOptions)
        .then((response) => response.json())
        .then((responseData)=>{return responseData;})
        .catch(error => console.warn(error));
}

export function getCurrentMoviesAndProjections(){
    const requestOptions = {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("jwt")}`,
        },
      };
  
      return fetch(
        `${serviceConfig.baseURL}/api/movies/current`, requestOptions
      )
        .then((response) => response.json())
        .then((responseData) => {return responseData;})
        .catch(error=>console.warn(error));
}

export function getAllMovies(){
  const requestOptions = {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("jwt")}`,
      },
    };

    return fetch(
      `${serviceConfig.baseURL}/api/movies`, requestOptions
    )
      .then((response) => response.json())
      .then((responseData) => {return responseData;})
      .catch(error=>console.warn(error));
}

export function getMovieById(movieId: string){
  const requestOptions = {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("jwt")}`,
      },
    };

    return fetch(
      `${serviceConfig.baseURL}/api/movies/${movieId}`, requestOptions
    )
      .then((response) => response.json())
      .then((responseData) => {return responseData;})
      .catch(error=>console.warn(error));
}

export function changeCurrentMovie(movieId: string){
  const requestOptions = {
      method: "PATCH",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("jwt")}`,
      },
    };

    return fetch(
      `${serviceConfig.baseURL}/api/movies/${movieId}`, requestOptions
    )
      .then((response) => response.json())
      .then((responseData) => {return responseData;})
      .catch(error=>console.warn(error));
}

export function DeleteMovieByid(movieId: string){
  const requestOptions = {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("jwt")}`,
      },
    };

    return fetch(
      `${serviceConfig.baseURL}/api/movies/${movieId}`, requestOptions
    )
      .then((response) => response.json())
      .then((responseData) => {return responseData;})
      .catch(error=>console.warn(error));
}

export function GetTagsByMovieIdAPI(movieId:string){
  const requestOptions = {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
  };

  return fetch(
    `${serviceConfig.baseURL}/api/moviewithtags/${movieId}`, requestOptions
  )
    .then((response) => response.json())
    .then((responseData) => {return responseData;})
    .catch(error=>console.warn(error));
}

export function GetMoviesByTagName(tag:string){
  const requestOptions = {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
  };

  return fetch(
    `${serviceConfig.baseURL}/api/movies/bytag/${tag}`, requestOptions
  )
    .then((response) => response.json())
    .then((responseData) => {return responseData;})
    .catch(error=>console.warn(error));  
}

export function GetMovieByTitle(title:string){
  const requestOptions = {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
  };

  return fetch(
    `${serviceConfig.baseURL}/api/movies/bytitle/${title}`, requestOptions
  )
    .then((response) => response.json())
    .then((responseData) => {return responseData;})
    .catch(error=>console.warn(error));  
}

export function GetMovieByYear(year:string){
  const requestOptions = {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
  };

  return fetch(
    `${serviceConfig.baseURL}/api/movies/byyear/${year}`, requestOptions
  )
    .then((response) => response.json())
    .then((responseData) => {return responseData;})
    .catch(error=>console.warn(error));  
}

export function AddMovie(movie:any){
  const requestOptions = {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
    body:JSON.stringify(movie),
  };

  return fetch(
    `${serviceConfig.baseURL}/api/movies`, requestOptions
  )
    .then((response) => response.json())
    .then((responseData) => {return responseData;})
    .catch(error=>console.warn(error));  
}

export function updateMovieAPI(id:any,movie:any){
  const requestOptions = {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
    body:JSON.stringify(movie),
  };

  return fetch(
    `${serviceConfig.baseURL}/api/movies/${id}`, requestOptions
  )
    .then((response) => response.json())
    .then((responseData) => {return responseData;})
    .catch(error=>console.warn(error));  
}

export function GetTopTenMovies(){
  const requestOptions = {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
  };

  return fetch(
    `${serviceConfig.baseURL}/api/movies/top10`, requestOptions
  )
    .then((response) => response.json())
    .then((responseData) => {return responseData;})
    .catch(error=>console.warn(error));  
}

export function GetTopTenMoviesByYearAPI(year:string){
  const requestOptions = {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
  };

  return fetch(
    `${serviceConfig.baseURL}/api/movies/top10/${year}`, requestOptions
  )
    .then((response) => response.json())
    .then((responseData) => {return responseData;})
    .catch(error=>console.warn(error));  
}

export function getProjections(){
  const requestOptions = {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
  };

  return fetch(`${serviceConfig.baseURL}/api/Projections`, requestOptions)
      .then((response) => response.json())
      .then((responseData) => {return responseData;})
      .catch((error) => {return error;})
}

export function deleteProjection(id:string){
  const requestOptions = {
    method: "DELETE",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
  };

  return fetch(`${serviceConfig.baseURL}/api/Projections/${id}`, requestOptions)
     .then((response) => response.json())
     .then((responseData) => {return responseData;})
     .catch((error) => {return error;});
}

export function editProjectionById(projection: any, projectionId: string)
{
  const requestOptions = {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
    body: JSON.stringify(projection),
  };

  return fetch(
    `${serviceConfig.baseURL}/api/projections/${projectionId}`, requestOptions
  )
    .then((response) => response.json())
    .then((responseData) => {return responseData;})
    .catch((error) => {return error;});
}

export function getAuditoriumsBySelectedCinema(selectedCinemaId: string){
    const requestOptions = {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("jwt")}`,
        },
      };
      return fetch(
        `${serviceConfig.baseURL}/api/Auditoriums/bycinemaid/${selectedCinemaId}`,
        requestOptions
      )
        .then((response) => response.json())
        .then((responseData)=> {return responseData;})
        .catch(error=>console.warn(error));
}


export function getProjectionById(id:string)
{
  const requestOptions = {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
  };
console.log(id);
  return fetch(`${serviceConfig.baseURL}/api/Projections/${id}`, requestOptions)
     .then((response) => response.json())
     .then((responseData) => {return responseData;})
     .catch((error) => {return error;});
}

export function getMoviesBySelectedAuditorium(selectedAuditoriumId: string){
    const requestOptions = {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("jwt")}`,
        },
      };

      console.log("here");
      return fetch(
        `${serviceConfig.baseURL}/api/movies/auditoriumId/${selectedAuditoriumId}`, requestOptions
      )
        .then((response) => response.json())
        .then((responseData) => {return responseData;})
        .catch(error=>console.log(error));
}

export function getCurrentMoviesFilteredReturnProjections(filterProj:IProjectionFilter){
    const requestOptions = {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("jwt")}`,
        },
        body: JSON.stringify(filterProj)
      };
      return fetch(
        `${serviceConfig.baseURL}/api/projections/filter`, requestOptions
      )
        .then((response) => response.json())
        .then((responseData) => {return responseData;});
}

export function addNewProjection(projection:any)
{
  const requestOptions = {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
    body: JSON.stringify(projection),
  };

  return fetch(`${serviceConfig.baseURL}/api/projections`, requestOptions)
    .then((response) => response.json())
    .then((responseData) => {return responseData;})
    .catch((error) => {return error;});
}

export function userLogIn(userName: string){
    const requestOptions = {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("jwt")}`,
        },
      };
      return fetch(
        `${serviceConfig.baseURL}/api/users/username/${userName}`, requestOptions)
        .then((response) => response.json())
        .then((responseData) => { return responseData;});
}

export function getUserToken(Role:string, userName:string){
    const requestOptions = {
        method: "GET",
      };
      return fetch(
        `${serviceConfig.baseURL}/get-token?role=${Role}&userName=${userName}`, requestOptions)
        .then((response) => response.json())
        .then((responseData) => {return responseData;});
}

export function tryPayment(){
  const requestOptions = {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
    body: "",
  };
  return fetch(`${serviceConfig.baseURL}/api/levi9payment`, requestOptions)
  .then((response) => response.json())
  .then((responseData) => {return responseData;})
  .catch((error) => {return error;});
}

export function getReservedSeatsByProjectionId(projectionId: string){
  const requestOptions = {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
  };
  return fetch(
    `${serviceConfig.baseURL}/api/tickets/busySeats/${projectionId}`,
    requestOptions
  )
   .then((response) => response.json())
   .then((responseData) => {return responseData;})
   .catch((error) => {return error;});
}

export function getSeatsByAuditoriumId(auditoriumId: string)
{
  const requestOptions = {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
  };

  return fetch(
    `${serviceConfig.baseURL}/api/seats/auditorium/${auditoriumId}`,
    requestOptions
  )
    .then((response) => response.json())
    .then((responseData) => {return responseData;})
    .catch((error) => {return error;});
}

export function makeReservationAPI(data:any){
  const requestOptions = {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
    body: JSON.stringify(data),
  };

  return fetch(`${serviceConfig.baseURL}/api/tickets`, requestOptions)
    .then((response) => response.json())
    .then((responseData) => {return responseData;})
    .catch((error) => {return error;});
}

export function checkIfTicketExistForProjection(id:string){
  const requestOptions = {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
  };
    return fetch(
      `${serviceConfig.baseURL}/api/tickets/deleteProjection/${id}`, requestOptions)
      .then((response) => response.json())
      .then((responseData) => {return responseData;});
}

export function getUserByUsernameFromAPI(userName: string)
{
  const requestOptions = {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("jwt")}`,
    },
  };

  return fetch(
    `${serviceConfig.baseURL}/api/users/username/${userName}`,
    requestOptions
  )
    .then((response) => response.json())
    .then((responseData) => {return responseData;})
    .catch((error) => {return error;});
}