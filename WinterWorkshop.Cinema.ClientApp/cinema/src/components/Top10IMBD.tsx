import React, { useEffect, useState } from "react";
import { NotificationManager } from "react-notifications";
import { serviceConfig } from "../appSettings";
import { Container, Row, Card, Table, Button } from "react-bootstrap";
import Spinner from "./Spinner";
import "./../index.css";
import { IMovieIMBD, IImage, IMovie } from "../models";
import { brotliDecompress } from "zlib";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
    faVideo
} from "@fortawesome/free-solid-svg-icons";

interface IState {
    movies: IMovieIMBD[];
    isLoading: boolean;
}

const Top10IMBD: React.FC = (props: any) => {
    const [state, setState] = useState<IState>({
        movies: [
            {
                id: "",
                bannerUrl: "",
                title: "",
                year: "",
                rating: 0,
                poster: "",
                plot: "",
                trailer: {
                    link: ""
                }
            },
        ],
        isLoading: true,
    });

    useEffect(() => {
        getTop10IMBDMovies();
    }, []);
    const moviesToShow: IMovieIMBD[] = [];

    const getTop10IMBDMovies = () => {
        const request = require('request');
        const options = {
            method: 'GET',
            url: 'https://imdb8.p.rapidapi.com/title/get-top-rated-movies',
            headers: {
                'x-rapidapi-key': '5669c8fcb1msh326ebc2d4117a74p17d199jsn2b71ed00d06b',
                'x-rapidapi-host': 'imdb8.p.rapidapi.com',
                useQueryString: true
            }
        };
        request(options, function (error, response, body) {
            if (JSON.parse(body).length === undefined) {
                NotificationManager.warning("IMBD is not current available, please try again letter");
            }
            else {
                if (JSON.parse(body).length > 10) {
                    let moviessz = JSON.parse(body).slice(0, 10);
                    for (let index = 0; index < moviessz.length; index++) {
                        getInfoForEachMovie(moviessz[index].id.split("/")[2]);
                    }
                }
            }
        });
    }

    const getInfoForEachMovie = (movieId: string) => {
        const request = require('request');

        const options = {
            method: 'GET',
            url: 'https://imdb-internet-movie-database-unofficial.p.rapidapi.com/film/' + movieId,
            headers: {
                'x-rapidapi-key': '5669c8fcb1msh326ebc2d4117a74p17d199jsn2b71ed00d06b',
                'x-rapidapi-host': 'imdb-internet-movie-database-unofficial.p.rapidapi.com',
                useQueryString: true
            }
        };

        request(options, function (error, response, body) {
            if (error) throw new Error(error);

            moviesToShow.push(JSON.parse(body));
            console.log(moviesToShow.length);
            if (moviesToShow.length == 10) {
                moviesToShow.sort((a, b) => a.rating > b.rating ? 1 : -1);
                setState((prevData) => ({ ...prevData, movies: moviesToShow }));
                setState((prevData) => ({ ...prevData, isLoading: false }));
            }
        });
    }
    const showAllTopMovies = () => {
        console.log(state.movies);
        return state.movies.map((movie) => {
            return (
                <Card.Body key={movie.id} className="imbdWidth">
                    <div className="imbdWidth">
                        <img className="imbdWidthPic, imbdHeightPic" title={movie.plot} src={movie.poster}></img>
                    </div>
                    <Card.Title>
                        <span className="card-title-font" >
                            {movie.title}
                        </span>
                    </Card.Title>
                    <Card.Subtitle className="mb-2 text-muted">
                        Year of production: {movie.year}
                    </Card.Subtitle>
                    <Card.Subtitle className="mb-2 text-muted">
                        Rating: {movie.rating}
                    </Card.Subtitle>
                    <Card.Subtitle className="mb-2 text-muted">

                        <a href={movie.trailer?.link}>  <FontAwesomeIcon className="text-primary mr-1" icon={faVideo} />Watch the trailer.</a>
                    </Card.Subtitle>
                </Card.Body>
            );
        });
    };
    return (
        <Container className="textAlignCenter">
            <Row className="text-info font-weight-bold text-capitalize navbar-brand">
                <h1 className="form-header form-heading projections-title">Top 10 Rated Movies on IMDb</h1>
            </Row>
            <Row>{state.isLoading ? <Spinner></Spinner> : showAllTopMovies()}</Row>
        </Container>
    );
};

export default Top10IMBD;
