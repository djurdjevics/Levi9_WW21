import React, { Component, useState } from 'react';
import { Button, Card, Container } from 'react-bootstrap';
import {IProjection} from "../../models";
import { withRouter } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
    faVideo
} from "@fortawesome/free-solid-svg-icons";
import { NotificationManager } from "react-notifications";

interface IState{
projections: IProjection[];
}

function AllCurrentProjections(props: any) {
    const [state, setState] = useState<IState>({
        projections: props.projections
    });


    const getRoundedRating = (rating: number) => {
        const result = Math.round(rating);
        return <span className="float-right">Rating: {result}/10</span>;
    };


    const navigateToProjectionDetails = (id: string, movieId: string) => {
        console.log("23");
        props.history.push(`projectiondetails/${id}/${movieId}`);
    };

    const showAllProjections = () => {
        console.log(props.projections);
        return props.projections.map((filteredProjection) => {
            return (
                <Card.Body key={filteredProjection.id}>
                    <div className="banner-img">
                        <img className="img-style" src={filteredProjection.movie.bannerUrl}></img>
                    </div>
                    <Card.Title>
                        <span className="card-title-font">
                            {filteredProjection.movieTitle} -{" "}
                            {filteredProjection.auditoriumName}
                        </span>
                        {filteredProjection.movie.rating &&
                            getRoundedRating(filteredProjection.movie.rating)}
                    </Card.Title>
                    <hr />
                    <Card.Subtitle className="mb-2 text-muted">
                        Year of production: {filteredProjection.movie.year}
                        {
                            (filteredProjection.movie.trailerUrl !== "" && filteredProjection.movie.trailerUrl !== null) && 
                            <span className="float-right" >   <a href={filteredProjection.movie.trailerUrl}> <FontAwesomeIcon className="text-primary mr-1" icon={faVideo} /> Watch Trailer Now </a></span>
                         
                        }
                        {
                            (filteredProjection.movie.trailerUrl === "" || filteredProjection.movie.trailerUrl === null) &&
                            <span className="float-right" >   <a > Trailer Not Available </a></span>
                         
                        }
                    </Card.Subtitle>
                    <hr />
                    <Card.Text>
                    <Button
                        key={filteredProjection.id}
                        onClick={() => navigateToProjectionDetails(
                            filteredProjection.id,
                            filteredProjection.movieId
                        )}
                        className="btn-projection-time"
                    >   Date: {filteredProjection.projectionTime.slice(0, 10)}&nbsp;
                        Time: {filteredProjection.projectionTime.slice(11, 16)}h
                    </Button>
                     
                    </Card.Text>
                 
                </Card.Body>
            );
        });
    };


    return (
        <Container>
            {showAllProjections()}
        </Container>
    );

}
 
export default withRouter(AllCurrentProjections);