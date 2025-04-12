import moment from "moment";
import { Fragment, useCallback, useEffect, useState } from "react";
import { Link, Outlet } from "react-router-dom";
import 'moment/locale/it';

interface GetRuolo{
    readonly username: string;
    readonly ruolo: string;
}

export function Layout(props: { setToken: (t: boolean) => void }) {
 const [, Data] = useState("");
 const [grado, setGrado] = useState("");
 const [nomeUtente, setNomeUtente] = useState("");
 const d = new Date();
 setTimeout(() => {
    Data(d.toString());
 }, 1000);
 //var lingua= moment.locale("it");
let data =moment().format(' HH:mm, DD/MM/Y');

const Logout = useCallback(async () => {
    let tt= await fetch("/Login/Logout", { method: "GET"});
    if(tt.status===200)
    {
    props.setToken(false);
    window.location.href="/";
    }
    
},[props]);

useEffect(() => {
    let isactive=true;
    const Autenticazione = async () => {
        let data = await fetch("/Login/Autenticazione", { method: "GET" , headers: {'Content-type': 'application/json; charl set=UTF-8'} });
        //let res = await data.json();
        if(isactive)
        {
            if (data.status !== 200) {
                props.setToken(false);
                window.location.reload();
            }
            setTimeout(()=>{
                Autenticazione();
            },5000);
        }
    };
    Autenticazione();
    return()=>{isactive=false;};
}, [props]);


useEffect(() => {
    let isActive = true;
    const fetchData = async () => {
        
        let data = await fetch("/Login/GetRuolo" , {method: 'GET'});
        if(!isActive) return;
        var res = await data.json() as GetRuolo;
        if(!isActive) return;
        setGrado(res.ruolo);
        setNomeUtente(res.username);
    };
    fetchData();
    return ()=>{isActive=false;}  //cleanup when component unmounts
},[grado]);


    return (
    <Fragment>
        <nav className="navbar navbar-expand-lg navbar-dark bg-dark iii">
            <div className="container-fluid">
                <div className="navbar-brand">Data {data}</div >
                <div className="navbar-brand">Benvenuto {nomeUtente}</div>
                    <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNavDarkDropdown" aria-controls="navbarNavDarkDropdown" aria-expanded="false" aria-label="Toggle navigation">
                    <span className="navbar-toggler-icon"></span>
                    </button>
                    <div className="collapse navbar-collapse" id="navbarNavDarkDropdown">
                        <ul className="navbar-nav">
                            <li className="nav-item dropdown">
                                <a className="nav-link dropdown-toggle Menu" href="/#" id="navbarDarkDropdownMenuLink" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                                Menu 
                                </a>
                                <ul className="dropdown-menu dropdown-menu-dark" aria-labelledby="navbarDarkDropdownMenuLink">
                                <li><Link to={"/"+(grado==="Admin"||grado==="Basic" ? "ESP":null)} className={""+(grado==="Admin"||grado==="Basic" ? "dropdown-item": null)}>{(grado==="Admin"||grado==="Basic" ? "ESP":null)}</Link></li>
                                <li><Link to={"/"+(grado==="Admin"||grado==="Basic" ?"CambiaPassword":null)} className={""+(grado==="Admin"||grado==="Basic" ? "dropdown-item": null)}>{(grado==="Admin"||grado==="Basic" ? "CambiaPassword":null)}</Link></li>
                                <li><Link to={"/"+(grado==="Admin"||grado==="root" ? "NuovoAccount": null)} className={""+(grado==="Admin"||grado==="root" ? "dropdown-item": null)}>{(grado==="Admin"||grado==="root" ? "NuovoAccount": null)}</Link></li>
                                <li><Link to={"/"+(grado==="Admin"||grado==="Basic" ? "Babylon": null)} className={""+(grado==="Admin"||grado==="Basic" ? "dropdown-item": null)}>{(grado==="Admin"||grado==="Basic" ? "Mappa3D": null)}</Link></li>
                                <li><Link to={"/"+(grado==="Admin" ? "ControlloUtenti": null)} className={""+(grado==="Admin" ? "dropdown-item": null)}>{(grado==="Admin" ? "ControlloUtenti": null)}</Link></li>
                                </ul>
                            </li>
                        </ul>
                        <button type="button" className=" btn btn-dark" onClick={Logout}>Logout</button>
                </div>
            </div>
        </nav>
        <Outlet/>
    </Fragment>
    );
}