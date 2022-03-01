import React, { useState } from "react";
import { withRouter } from "react-router-dom";
import {
  FormGroup,
  FormControl,
  Button,
  Container,
  Row,
  Col,
  FormText,
} from "react-bootstrap";
import { NotificationManager } from "react-notifications";
import { serviceConfig } from "../../../appSettings";
import { YearPicker } from "react-dropdown-date";
import { AddMovie } from "../../APICommunication";

interface IState {
  title: string;
  year: string;
  rating: string;
  current: boolean;
  titleError: string;
  hasOscar: boolean;
  submitted: boolean;
  canSubmit: boolean;
  tags: string;
  bannerUrl: string;
  yearError: string;
  trailerUrl: string;
}

const NewMovie: React.FC = (props: any) => {
  const [state, setState] = useState<IState>({
    title: "",
    year: "",
    rating: "",
    current: false,
    titleError: "",
    submitted: false,
    canSubmit: true,
    hasOscar: false,
    tags: "",
    bannerUrl: "",
    yearError: "",
    trailerUrl: ""
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target;
    setState((prevData)=> ({...prevData,[id]: value}));
    validate(id, value);
  };

  const handleTagsChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    let val = e.target.value;
    setState((prevData)=> ({...prevData, tags: val }));
  };

  const handleBannerUrlChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    let val = e.target.value;
    setState((prevData)=> ({...prevData, bannerUrl: val}));
  };

  const validate = (id: string, value: string) => {
    if (id === "title") {
      if (value.trim() === "") {
        setState((prevData)=> ({...prevData, title: value,
          titleError: "Fill in movie title",
          canSubmit: false}));
      } else {
        setState((prevData)=> ({...prevData,title: value, titleError: "", canSubmit: true }));
      }
    }

    if (id === "year") {
      const yearNum = +value;
      if (!value || value === "" || yearNum < 1895 || yearNum > 2100) {
        setState((prevData)=> ({...prevData,year:value, yearError: "Please chose valid year" }));
      } else {
        setState((prevData)=> ({...prevData, year: value, yearError: "" }));
      }
    }
  };

  const getTrailerFromAPI = (title:string, splitTags:any) => {
    const request = require('request');

    const options = {
      method: 'GET',
      url: 'https://imdb-internet-movie-database-unofficial.p.rapidapi.com/film/' + title,
      headers: {
        'x-rapidapi-key': '5669c8fcb1msh326ebc2d4117a74p17d199jsn2b71ed00d06b',
        'x-rapidapi-host': 'imdb-internet-movie-database-unofficial.p.rapidapi.com',
        useQueryString: true
      }
    };
    
    request(options, function (error, response, body) {
      if (error) throw new Error(error);     
      let data = JSON.parse(body).trailer?.link;
      setState((prevData)=> ({...prevData, trailerUrl:data}));
      addMovie(splitTags, data);
    });
  }

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    let splitTags = state.tags.split(",");
    setState((prevData)=> ({...prevData,submitted: true}));
    const { title, year, rating } = state;
    if (title && year && rating && splitTags[0] !== "") {
      getTrailerFromAPI(title, splitTags);
    } else {
      NotificationManager.error("Please fill in data");
      setState((prevData)=> ({...prevData,  submitted: false }));
    }
  };

  const handleYearChange = (year: string) => {
    setState((prevData)=> ({...prevData,  year: year}));
    validate("year", year);
  };

  const addMovie = (splitTags: string[], trailerUrl: string) => {
    console.log(state.trailerUrl);
    const data = {
      Title: state.title,
      Year: +state.year,
      Current: state.current,
      HasOscar: state.hasOscar,
      Rating: +state.rating,
      Tags: splitTags,
      BannerUrl: state.bannerUrl,
      TrailerUrl: trailerUrl
    };
  AddMovie(data).then((response)=>{
      if(response){
        NotificationManager.success("Successfully added movie!");
        props.history.push(`AllMovies`);
      }
      else{
        console.log(response);
        NotificationManager.error(response.message || response.statusText);
        setState((prevData)=>({...prevData, submitted:false}));
      }
    })

  };

  return (
    <Container>
      <Row>
        <Col>
          <h1 className="form-header">Add New Movie</h1>
          <form onSubmit={handleSubmit}>
            <FormGroup>
              <FormControl
                id="title"
                type="text"
                placeholder="Movie Title"
                value={state.title}
                onChange={handleChange}
                className="add-new-form"
              />
              <FormText className="text-danger">{state.titleError}</FormText>
            </FormGroup>
            <FormGroup>
              <YearPicker
                defaultValue={"Select Movie Year"}
                start={1895}
                end={2021}
                reverse
                required={true}
                //disabled={false}
                value={state.year}
                onChange={(year: string) => {
                  handleYearChange(year);
                }}
                id={"year"}
                name={"year"}
                classes={"form-control add-new-form"}
                optionClasses={"option classes"}
              />
              <FormText className="text-danger">{state.yearError}</FormText>
            </FormGroup>
            <FormGroup>
              <FormControl
                as="select"
                className="add-new-form"
                placeholder="Rating"
                id="rating"
                value={state.rating}
                onChange={handleChange}
              >
                <option value="1">1</option>
                <option value="2">2</option>
                <option value="3">3</option>
                <option value="4">4</option>
                <option value="5">5</option>
                <option value="6">6</option>
                <option value="7">7</option>
                <option value="8">8</option>
                <option value="9">9</option>
                <option value="10">10</option>
              </FormControl>
            </FormGroup>
            <FormGroup>
              <FormControl
                className="add-new-form"
                as="select"
                placeholder="Current"
                id="current"
                value={state.current.toString()}
                onChange={handleChange}
              >
                <option value="true">Current</option>
                <option value="false">Not Current</option>
              </FormControl>
            </FormGroup>

            <FormGroup>
              <FormControl
                className="add-new-form"
                as="select"
                placeholder="Has Oscar"
                id="hasOscar"
                value={state.hasOscar.toString()}
                onChange={handleChange}
              >
                <option value="true">Has oscar</option>
                <option value="false">Doesn't have oscar</option>
              </FormControl>
            </FormGroup>

            <FormControl
              id="tags"
              type="text"
              placeholder="Movie Tags"
              value={state.tags}
              onChange={(e: React.ChangeEvent<HTMLInputElement>) => {
                handleTagsChange(e);
              }}
              className="add-new-form"
            />
            <FormControl
              id="bannerUrl"
              type="text"
              placeholder="Banner Url"
              value={state.bannerUrl}
              onChange={(e: React.ChangeEvent<HTMLInputElement>) => {
                handleBannerUrlChange(e);
              }}
              className="add-new-form"
            />
            <FormText className="text-danger">{state.titleError}</FormText>
            <Button
              className="btn-add-new"
              type="submit"
              disabled={state.submitted || !state.canSubmit}
              block
            >
              Add
            </Button>
          </form>
        </Col>
      </Row>
    </Container>
  );
};

export default withRouter(NewMovie);
