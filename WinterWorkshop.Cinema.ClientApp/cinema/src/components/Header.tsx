import React, { useEffect, useState } from "react";
import { withRouter } from "react-router-dom";
import { Link } from "react-router-dom";
import { Navbar, Nav, Form, FormControl, Button } from "react-bootstrap";
import { NotificationManager } from "react-notifications";
import { serviceConfig } from "../appSettings";
import {
  getTokenExp,
  isGuest,
  getUserName,
} from "../../src/components/helpers/authCheck";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
    faVideo,
    faCrown,
    faGrinBeam
} from "@fortawesome/free-solid-svg-icons";
import { userLogIn, getUserToken, getUserByUsernameFromAPI } from "./APICommunication";

import { UserAddOutlined, LoginOutlined, HomeOutlined  } from '@ant-design/icons';
import { MailOutlined, FormOutlined  , VideoCameraOutlined ,RedditOutlined, QqOutlined, CrownOutlined } from '@ant-design/icons';
import { Menu, Dropdown } from 'antd';
import { DownOutlined } from '@ant-design/icons';
interface IState {
  username: string;
  submitted: boolean;
  token: boolean;
  shouldHide: boolean;
  role: string;
  userPoints: number;
}

const Header: React.FC = (props: any) => {
  const [state, setState] = useState<IState>({
    username: "",
    submitted: false,
    token: false,
    shouldHide: true,
    role: "",
    userPoints: 0
  });

  useEffect(() => {
    if (getUserName() !== "guest") { //ova provera je bolja da ne bi neko u browseru sam setovao "userLoggedIn" na true
      // if (localStorage.getItem("userLoggedIn") !== null) {
      getUserByUsernameFromAPI(getUserName()).then((data)=>{
        if(data != null)
        {
          setState((prevData)=>({...prevData, userPoints:data.bonusPoints}));
        }
      })
      hideLoginButtonElement();
    } else {
      hideLogoutButtonElement();
    }
  }, []);

  let shouldDisplayUserProfile = false;
  const shouldShowUserProfile = () => {
    shouldDisplayUserProfile = !isGuest();
    return shouldDisplayUserProfile;
  };

  useEffect(() => {
    let tokenExp = getTokenExp();
    let currentTimestamp = +new Date();
    if (!tokenExp || tokenExp * 1000 < currentTimestamp) {
      getTokenForGuest();
    }
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target;
    setState({ ...state, [id]: value });
  };

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    setState({ ...state, submitted: true });
    const { username } = state;
    if (username) {
      login();
    } else {
      setState({ ...state, submitted: false });
    }
  };

  const handleSubmitLogout = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    // localStorage.removeItem("userLoggedIn");
    setState({ ...state, submitted: true });
    setState({ ...state, token: false });
    getTokenForGuest();
  };

  const hideLoginButtonElement = () => {
    let loginButton = document.getElementById("login");
    if (loginButton) {
      loginButton.style.display = "none";
    }
    let logoutButton = document.getElementById("logout");
    if (logoutButton) {
      logoutButton.style.display = "block";
    }
    document.getElementById("username")!.style.display = "none";
  };

  const hideLogoutButtonElement = () => {
    let loginButton = document.getElementById("login");

    if (loginButton) {
      loginButton.style.display = "block";
    }
    let logoutButton = document.getElementById("logout");
    if (logoutButton) {
      logoutButton.style.display = "none";
    }
    document.getElementById("username")!.style.display = "block";
  };

  const login = () => {
    // localStorage.setItem("userLoggedIn", "true");
    if (state.username === "." || state.username === ".." || state.username.trim() === "") {
      NotificationManager.error("Username cannot be .(dot) or space.");
    }
    else {
      userLogIn(state.username).then((data) => {
        setState({ ...state, token: true });
        if (data.userName) {
          getToken(data.role);
          NotificationManager.success(`Welcome, ${data.firstName}!`);
          setState((prevData)=>({...prevData, shouldHide:false}));
        }
        else {
          // localStorage.removeItem("userLoggedIn");
          NotificationManager.error(data.errorMessage);
          setState({ ...state, submitted: false });
        }
      })
        .catch((response) => {
          // localStorage.removeItem("userLoggedIn");
          NotificationManager.error(response.errorMessage);
          setState({ ...state, submitted: false });
        });
    }
  };

  const getToken = (
    Role: string
  ) => {
    getUserToken(Role, state.username).then((data) => {
      if (data.token) {
        localStorage.setItem("jwt", data.token);
        setTimeout(() => {
          window.location.reload();
        }, 500);
      }
    })
      .catch((response) => {
        NotificationManager.error(response.message || response.statusText);
        setState({ ...state, submitted: false });
      });
  };

  const getTokenForGuest = () => {
    let role = "guest";
    let username = "guest"
    getUserToken(role, username).then((data) => {
      if (data.token) {
        localStorage.setItem("jwt", data.token);
        setTimeout(() => {
          window.location.reload();
        }, 500);
      }
    })
      .catch((response) => {
        NotificationManager.error(response.message || response.statusText);
        setState({ ...state, submitted: false });
      });
    state.token = true;
  };

  const redirectToUserPage = () => {
    props.history.push(`/dashboard/userprofile`);
  };

  const redirectToSignUp = () => {
    props.history.push(`Signup`);
  }

  const refreshPage = () => {
    window.location.reload(true);
  };

  const shouldShowLogIn = () => {
    return false;
  }

  return (
    <Navbar bg="dark" expand="lg">
      <Navbar.Brand className="text-info font-weight-bold text-capitalize">
      <Link className="text-decoration-none" to="/dashboard/Projection">
        intELLIGENCE<RedditOutlined  className="main-icon mr-2 ml-2 fa-1x" />CINEMA
      
      </Link>
      </Navbar.Brand>
      <Navbar.Toggle aria-controls="basic-navbar-nav" className="text-white" />
      <Navbar.Collapse id="basic-navbar-nav" className="text-white">
        <Nav className="mr-auto text-white"></Nav>
        <Form
          inline
          onSubmit={(e: React.FormEvent<HTMLFormElement>) => handleSubmit(e)}
        >
          <FormControl
            type="text"
            placeholder="Username"
            id="username"
            value={state.username}
            onChange={handleChange}
            className="mr-sm-2"
          />
          <Button type="submit" className="mr-2" variant="outline-success" id="login">
            Sign in
          </Button>
        </Form>
        {!shouldShowUserProfile() && (
          <Button onClick={redirectToSignUp} type="submit" variant="outline-primary" id="signUp">
            Sign up
          </Button>
        )}

        {shouldShowUserProfile() && (
          <Button
            style={{ backgroundColor: "transparent", marginRight: "10px" }}
            onClick={redirectToUserPage}
          >
            {getUserName()}
          </Button>
        )}
        {
          shouldShowUserProfile() && (
            <Button style={{ backgroundColor: "transparent", marginRight: "10px" }} title="Bonus points">
              {state.userPoints} &nbsp;<FontAwesomeIcon className="text-primary" style={{fontSize:"20px"}} icon={faCrown} />
            </Button>
          )
        }
        <Form
          inline
          onSubmit={(e: React.FormEvent<HTMLFormElement>) =>
            handleSubmitLogout(e)
          }
        >
          <Button type="submit" variant="outline-danger" id="logout">
            Logout
          </Button>
        </Form>
      </Navbar.Collapse>
    </Navbar>
  );
};

export default withRouter(Header);
