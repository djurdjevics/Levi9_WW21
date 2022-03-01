import React, { useEffect, useState } from "react";
import { NotificationManager } from "react-notifications";
import { serviceConfig } from "../../appSettings";
import { Row } from "react-bootstrap";
import { getUserName, getRole } from "../helpers/authCheck";
import { withRouter } from "react-router";
import { IProjection, IUser, IReservation } from "../../models";

import { getUserByUsernameFromAPI } from "../APICommunication";

interface IState {
  user: IUser;
  reservations: IReservation[];
  projection: IProjection[];
  submitted: boolean;
}

const UserProfile: React.FC = () => {
  const [state, setState] = useState<IState>({
    user: {
      id: "",
      firstName: "",
      lastName: "",
      bonusPoints: "",
    },
    reservations: [
      {
        projectionId: "",
      },
    ],
    projection: [],
    submitted: false,
  });

  useEffect(() => {
    getUserByUsername();
  }, []);

  const getUserByUsername = () => {
    let userName = getUserName();

    getUserByUsernameFromAPI(userName).then((data)=>{
      if (data) {
        setState({ ...state, user: data });
      }
    })
    .catch((response) => {
      NotificationManager.error(response.message || response.statusText);
      setState({ ...state, submitted: false });
    });
  };

  return (
    <React.Fragment>
      <Row className="no-gutters pt-2 align-center justify-content-center">
        <h1 className="form-header form-heading align-center justify-content-center blueishText">
          Hello, {state.user.firstName}!
        </h1>
      </Row>
      <Row className="no-gutters pr-5 pl-5align-center justify-content-center">
        <div className="card mb-3 user-info-container">
          <div className="row no-gutters">
            <div className="avatar-img2 col-md-4">
              <img
                className="avatar-img avatar-img2"
                alt="..."
              />
            </div>
            <div className="col-md-8">
              <div className="card-body align-center justify-content-center">
                <h5 className="card-title">User details:</h5>
                <p className="card-text">
                  <strong>Full name:</strong>{" "}
                  {`${state.user.firstName} ${state.user.lastName}`}
                </p>
                <p className="card-text">
                  <strong>Bonus points: </strong> {state.user.bonusPoints}
                </p>
                <p className="card-text">
                  <strong>Status: </strong> {getRole()}
                </p>
              </div>
            </div>
          </div>
        </div>
      </Row>
    </React.Fragment>
  );
};

export default withRouter(UserProfile);
