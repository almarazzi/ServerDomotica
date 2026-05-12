import moment from "moment";
import { Fragment, useCallback, useEffect, useState } from "react";
import { Link, Outlet } from "react-router-dom";


interface GetRuolo {
    readonly username: string;
    readonly ruolo: string;
}

export function Layout(props: { setToken: (t: boolean) => void; p: boolean; Grade: GetRuolo }) {
    const [, Data] = useState("");
    const [grado, setGrado] = useState("");
    const [nomeUtente, setNomeUtente] = useState("");
    const d = new Date();
    setTimeout(() => {
        Data(d.toString());
    }, 1000);
    let data = moment().format('HH:mm, DD/MM/Y');

    const Logout = useCallback(async () => {
        let tt = await fetch("/Login/Logout", { method: "GET" });
        if (tt.status === 200) {
            props.setToken(false);
            window.location.href = "/";
        }

    }, [props]);
    useEffect(() => {
        setGrado(props.Grade.ruolo);
        setNomeUtente(props.Grade.username);
    }, [props.Grade]);


   
    return (
        <Fragment>
            <nav className="navbar navbar-expand-lg navbar-dark bg-dark iii">
                <div className="container-fluid ">
                    <div className="prova1234">
                        <div className="navbar-brand">{data}</div >
                        <div className="navbar-brand">Benvenuto/a {nomeUtente}</div>
                        <button type="button" className=" btn btn-dark" onClick={Logout}>Logout</button>
                    </div>
                    <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNavDarkDropdown" aria-controls="navbarNavDarkDropdown" aria-expanded="false" aria-label="Toggle navigation">
                        <span className="navbar-toggler-icon"></span>
                    </button>
                    <div className="collapse navbar-collapse" id="navbarNavDarkDropdown">
                        <ul className="navbar-nav">
                            <li className="nav-item dropdown">
                                <a className="nav-link dropdown-toggle Menu" id="navbarDarkDropdownMenuLink" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    Menu
                                </a>
                                <ul className="dropdown-menu dropdown-menu-dark" aria-labelledby="navbarDarkDropdownMenuLink">
                                    {(grado === "Admin" || grado === "Basic") && (
                                        <>
                                            <li>
                                                <Link to={"/ESP"} className={"dropdown-item"}>ESP</Link>
                                            </li>
                                            <li>
                                                <Link to={"/CambiaPassword"} className={"dropdown-item"}>CambiaPassword</Link>
                                            </li>
                                            
                                            {props.p === false &&(
                                                <>
                                                    <li>
                                                        <Link to={"/Babylon"} className={"dropdown-item"}>Mappa3D</Link>
                                                    </li>
                                                </>
                                            )}
                                        </>
                                    )}
                                      {(grado === "Admin" || grado === "root") && (
                                        <>
                                            <li>
                                                <Link to={"/NuovoAccount"} className={"dropdown-item"}> NuovoAccount</Link>
                                            </li>
                                        </>
                                    )}
                                    {(grado === "Admin") && (
                                    <>
                                        {props.p === false &&(
                                            <li>
                                                <Link to={"/ControlloUtenti"} className={"dropdown-item"}>ControlloUtenti</Link>
                                                <Link to={"/DiagnosticaServer"} className={"dropdown-item"}>DiagnosticaServer</Link>
                                            </li>
                                        )}
                                    </>
                                    )}
                                </ul>
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>
            <Outlet/>
        </Fragment>
    );
}
export default Layout;
