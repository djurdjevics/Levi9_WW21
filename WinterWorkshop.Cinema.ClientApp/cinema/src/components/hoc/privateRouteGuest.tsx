import React, { useEffect } from "react";
import { Route, Redirect } from "react-router-dom";
import { NotificationManager } from "react-notifications";
import * as authCheck from "../helpers/authCheck";

export const PrivateRouteGuest = ({ component: Component, ...rest }) => {
  useEffect(() => {
    if (!authCheck.isGuest()) {
      NotificationManager.error("You shall not pass!");
    }
  });

  return (
    <Route
      {...rest}
      render={(props) =>
        localStorage.getItem("jwt") && authCheck.isGuest() ? (
          <Component {...props} />
        ) : (
          <Redirect to={{ pathname: "/", state: { from: props.location } }} />
        )
      }
    />
  );
};
