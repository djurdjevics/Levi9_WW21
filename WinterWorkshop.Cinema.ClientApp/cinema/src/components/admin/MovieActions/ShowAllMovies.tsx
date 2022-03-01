import React, { useEffect, useState } from "react";
import { NotificationManager } from "react-notifications";
import { serviceConfig } from "../../../appSettings";
import { Row, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faEdit,
  faTrash,
  faInfoCircle,
  faLightbulb,
  faWindowRestore,
} from "@fortawesome/free-solid-svg-icons";
import Spinner from "../../Spinner";
import "./../../../index.css";
import {
  isAdmin,
  isSuperUser,
  isUser,
  isGuest,
} from "./../../helpers/authCheck";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { IMovie, ITag } from "../../../models";
import { getAllMovies, getCurrentMoviesAndProjections,changeCurrentMovie, DeleteMovieByid, GetTagsByMovieIdAPI, GetMovieByTitle, GetMoviesByTagName, GetMovieByYear } from "../../APICommunication";

interface IState {
  movies: IMovie[];
  tags: ITag[];
  title: string;
  year: string;
  id: string;
  rating: number;
  current: boolean;
  tag: string;
  listOfTags: string[];
  titleError: string;
  yearError: string;
  submitted: boolean;
  isLoading: boolean;
}

const ShowAllMovies: React.FC = (props: any) => {
  const [state, setState] = useState<IState>({
    movies: [
      {
        id: "",
        bannerUrl: "",
        title: "",
        year: "",
        current: false,
        rating: 0,
      },
    ],
    tags: [
      // {
      //   tagId: "",
      //   tagName: "",
      //   movieId: "",
      //   movieTitle: ""
      // },
    ],
    title: "",
    year: "",
    id: "",
    rating: 0,
    current: false,
    tag: "",
    listOfTags: [],
    titleError: "",
    yearError: "",
    submitted: false,
    isLoading: true,
  });

  toast.configure();

  let userShouldSeeWholeTable;
  const shouldUserSeeWholeTable = () => {
    if (userShouldSeeWholeTable === undefined) {
      userShouldSeeWholeTable = !isGuest() && !isUser();
    }
    return userShouldSeeWholeTable;
  };

  useEffect(() => {
    getProjections();
  }, []);

  const getMovie = (movieId: string) => {
    const requestOptions = {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("jwt")}`,
      },
    };

    fetch(`${serviceConfig.baseURL}/api/movies/${movieId}`, requestOptions)
      .then((response) => {
        if (!response.ok) {
          return Promise.reject(response);
        }
        return response.json();
      })
      .then((data) => {
        if (data) {
          setState((prevData)=>({
            ...prevData,
            title: data.title,
            year: data.year,
            rating: Math.round(data.rating),
            current: data.current === true,
            id: data.id,
          }));
        }
      })
      .catch((response) => {
        NotificationManager.error(response.message || response.statusText);
        setState((prevData)=>({ ...prevData, submitted: false }));
      });
  };

  const changeCurrent = (
    e: React.MouseEvent<HTMLTableDataCellElement, MouseEvent>,
    id: string
  ) => {
    e.preventDefault();
    changeCurrentMovie(id).then((data)=>{
      if(data){
        NotificationManager.success("Successfully changed current status for movie!");
        const newState = state.movies.filter((movie)=>{
          return movie.id !== id;
        });
        setState((prevData)=>({...prevData,movies:newState}));
        setTimeout(()=>{
          window.location.reload();
        },2000);
      }
      else{
        NotificationManager.error("This movie has projections!");
        setState((prevData)=>({...prevData,submitted:false}));
      }
    })
  };

  const getProjections = () => {
    if (isAdmin() === true || isSuperUser() === true) {
      setState((prevData)=>({ ...prevData, isLoading: true }));

      getAllMovies().then((data)=>{
        if(data){
          setState((prevData)=>({...prevData,movies:data,isLoading:false}));
        }
        else{
          setState((prevData)=>({ ...prevData, isLoading: false }));
          NotificationManager.error(data.message || data.statusText);
          setState((prevData)=>({ ...prevData, submitted: false }));
        }
      })
    } else {
      setState((prevData)=>({...prevData,isLoading:true}));
      getCurrentMoviesAndProjections().then((data)=>{
        if(data){
          setState((prevData)=>({...prevData,isLoading:false,movies:data}));
        }
        else{
           setState((prevData)=>({ ...prevData, isLoading: false }));
           NotificationManager.error(data.message || data.statusText);
           setState((prevData)=>({ ...prevData, submitted: false }));          
        }
      })
    }
  };

  const removeMovie = (id: string) => {

    DeleteMovieByid(id).then((data)=>{
      console.log(data);
      if(data.statusCode != 400){
        NotificationManager.success("Successfully removed movie!");
        const newState = state.movies.filter((movie)=>{
          return movie.id !== id;
        })
        setState((prevData)=>({...prevData,movies:newState}));
      }
      else{
        NotificationManager.error(data.errorMessage||data.statusText);
        setState((prevData)=>({...prevData,submitted:false}));
      }
    })
  };

  const getTagsByMovieId = (
    e: React.MouseEvent<HTMLTableDataCellElement, MouseEvent>,
    movieId: string
  ) => {
    e.preventDefault();

    GetTagsByMovieIdAPI(movieId).then((data)=>{
      if(data){
        showTags(data);
      }
      else{
        NotificationManager.error(data.message || data.statusText);
        setState((prevData)=>({...prevData,submitted:false}));
      }
    })
  }
  const showTags = (data:any) => {
    setState((prevData)=>({...prevData,listOfTags:[]}));
    setState((prevData)=>({...prevData,tags:[]}));
    data.map((tag)=>{
      state.tags.push(tag);
    })

    state.tags.map((tag)=>{
      console.log(tag);
      state.listOfTags.push(tag.tagName);
    })

    var list = " | ";
     for (var i = 0; i < state.listOfTags.length; i++) {
       list += state.listOfTags[i] + " | ";
    }
    toast.info(list,{
      position: toast.POSITION.BOTTOM_CENTER,
      className: "toast-class",
    });
  };

  const fillTableWithDaata = () => {
    return state.movies.map((movie) => {
      return (
        <tr key={movie.id}>
          <td
            className="text-center cursor-pointer"
            onClick={(
              e: React.MouseEvent<HTMLTableDataCellElement, MouseEvent>
            ) => getTagsByMovieId(e, movie.id)}
          >
            <FontAwesomeIcon
              className="text-info mr-2 fa-1x"
              icon={faInfoCircle}
            />
          </td>
          <td>{movie.title}</td>
          <td>{movie.year}</td>
          <td>{Math.round(movie.rating)}/10</td>

          {shouldUserSeeWholeTable() && <td>{movie.current ? "Yes" : "No"}</td>}
          {shouldUserSeeWholeTable() && (
            <td
              className="text-center cursor-pointer"
              onClick={() => editMovie(movie.id)}
            >
              <FontAwesomeIcon className="text-info mr-2 fa-1x" icon={faEdit} />
            </td>
          )}
          {shouldUserSeeWholeTable() && (
            <td
              className="text-center cursor-pointer"
              onClick={() => removeMovie(movie.id)}
            >
              <FontAwesomeIcon
                className="text-danger mr-2 fa-1x"
                icon={faTrash}
              />
            </td>
          )}
          {shouldUserSeeWholeTable() && (
            <td
              className="text-center cursor-pointer"
              onClick={(
                e: React.MouseEvent<HTMLTableDataCellElement, MouseEvent>
              ) => changeCurrent(e, movie.id)}
            >
              <FontAwesomeIcon
                className={
                  movie.current
                    ? "text-warning mr-2 fa-1x"
                    : "text-secondary mr-2 fa-1x"
                }
                icon={faLightbulb}
              />
            </td>
          )}
        </tr>
      );
    });
  };

  const editMovie = (id: string) => {
    props.history.push(`editmovie/${id}`);
  };

  const handleChange = (e) => {
    const { id, value } = e.target;
    setState((prevData)=>({ ...prevData, [id]: value }));
  };

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    var data = new FormData(e.currentTarget);

    var queryParts: string[] = [];
    var entries = data.entries();

    for (var pair of entries) {
      queryParts.push(
        `${encodeURIComponent(pair[0])}=${encodeURIComponent(
          pair[1].toString()
        )}`
      );
    }
    var query = queryParts.join("&");
    var loc = window.location;
    var url = `${loc.protocol}//${loc.host}${loc.pathname}?${query}`;

    let tag = url.split("=")[1];

    setState((prevData)=>({ ...prevData, submitted: true }));
    if (tag) {
      searchMovie(tag);
    } else {
      NotificationManager.error(
        "Please type type something in search bar to search for movies."
      );
      setState((prevData)=>({ ...prevData, submitted: false }));
    }
  };

  const searchMovie = (tag: string) => {
    setState((prevData)=>({...prevData,isLoading:true}));
    GetMoviesByTagName(tag).then((data)=>{
      if(data.length==0){
        GetMovieByTitle(tag).then((title)=>{
          if(title.length==0){
            GetMovieByYear(tag).then((year)=>{
              if(year.length==0){
                NotificationManager.error("Movie doesn't exist!");
                setState((prevData)=>({...prevData,isLoading:false}));
              }
              else{
                setState((prevData)=>({...prevData,movies:year,isLoading:false}));
              }
            })
          }
          else{
            setState((prevData)=>({...prevData,movies:title,isLoading:false}));
          }
        })
      }
      else{
        setState((prevData)=>({...prevData,movies:data,isLoading:false}));
      }
    });
    
  };

  let inputValue;
  const rowsData = fillTableWithDaata();
  const table = (
    <Table striped bordered hover size="sm" variant="dark">
      <thead>
        <tr>
          <th>Tags</th>
          <th>Title</th>
          <th>Year</th>
          <th>Rating</th>
          {shouldUserSeeWholeTable() && <th>Is Current</th>}
          {shouldUserSeeWholeTable() && <th></th>}
          {shouldUserSeeWholeTable() && <th></th>}
        </tr>
      </thead>
      <tbody>{rowsData}</tbody>
    </Table>
  );
  const showTable = state.isLoading ? <Spinner></Spinner> : table;

  return (
    <React.Fragment>
      <Row className="no-gutters pt-2">
        <h1 className="form-header form-heading">All Movies</h1>
      </Row>
      <Row>
        <form
          onSubmit={(e: React.FormEvent<HTMLFormElement>) => handleSubmit(e)}
          className="form-inline search-field md-form mr-auto mb-4 search-form search-form-second"
        >
          <input
            className="mr-sm-2 search-bar"
            id="tag"
            type="text"
            placeholder="Search"
            name="inputValue"
            value={inputValue}
            aria-label="Search"
          />
          <button className="btn-search" type="submit">
            Search
          </button>
        </form>
      </Row>
      <Row className="no-gutters pr-5 pl-5">{showTable}</Row>
    </React.Fragment>
  );
};

export default ShowAllMovies;
