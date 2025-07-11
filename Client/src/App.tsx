import './App.css';
import { Routes, Route, HashRouter, Navigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import  Layout  from './componenti/Layout';
import  Automatico  from './componenti/Automatico';
import  Manuale  from './componenti/Manuale';
import  Signin  from './componenti/Signin';
import  CambiaPassword  from './componenti/CambiaPassword';
import  NuovoAccount  from './componenti/NuovoAccount';
import  ControlloUtenti  from './componenti/ControlloUtenti';
import  Esp  from './componenti/Esp';
import  Babylon  from './componenti/Babylon';


interface Lista {
  readonly nomeEspClient: string;
  readonly ipEsp: string;
  readonly abilitazione: boolean;
}
interface key {
  key: string;
  value: Lista;
}

interface GetRuolo {
  readonly ruolo: string;
  readonly username: string;
}
function App() {
  const [token, setToken] = useState(false);
  const [SizeG, setSizeG] = useState(false);
  const [grado, setGrado] = useState("");
  const [Getgrado, setGetGrado] = useState<GetRuolo>({} as GetRuolo);
  const [lista, Setlista] = useState([] as key[]);
  useEffect(() => {
    let isActive = true;
    const fetchData = async () => {
      let data = await fetch("/Login/GetRuolo", { method: 'GET' });
      if (!isActive) return;
      var res = await data.json() as GetRuolo;
      if (!isActive) return;
      setGetGrado(res);
    };
    fetchData();
    return () => { isActive = false; }  
  }, [token]);

  useEffect(()=>{
    setGrado(Getgrado.ruolo);
  },[Getgrado]);

  useEffect(() => {
    let isActive = true;
    const fetchData = async () => {
      let data = await fetch("/apiEsp/ListaEsp", { method: 'GET' });
      if (!isActive) return;
      var res = await data.json() as key[];
      if (!isActive) return;
      Setlista(res);

      if (isActive === true) {
        setTimeout(() => {
          fetchData();
        }, 500);
      }
    };
    fetchData();
    return () => { isActive = false; }
  }, [token]);


  useEffect(() => {
    const Autenticazione = async () => {
      let data = await fetch("/Login/Autenticazione", { method: "GET"});
      if (data.status === 200) {
        setToken(true);
      } else {
        setToken(false);
      }
      setTimeout(() => {
        Autenticazione();
      }, 1000)
    };
    Autenticazione();
  }, []);

  useEffect(() => {
    const larghezza = () => {
      if (window.innerWidth <= 1000) {
        setSizeG(true);
      } else {
        setSizeG(false);
      }
      setTimeout(() => {
        larghezza();
      }, 1000);
    }
    larghezza();
  }, [])


  
  return (
    <div>
      <HashRouter>
        <Routes>
          <Route path="/Babylon" element={(grado === "Admin" || grado === "Basic" ? <Babylon mac={lista} /> : null)} />
          <Route path="/" element={(token === true ? <Layout setToken={setToken} p={SizeG} Grade={Getgrado} /> : <Signin setToken={setToken} />)}>
            <Route index element={<Navigate to="/ESP" />} />
            <Route path="/CambiaPassword" element={(grado === "Admin" || grado === "Basic" ? <CambiaPassword /> : null)} />
            <Route path="/ESP" element={(grado === "Admin" || grado === "Basic" ? <Esp lista={lista}/> : null)} />
            <Route path="/NuovoAccount" element={(grado === "Admin" || grado === "root" ? <NuovoAccount /> : null)} />
            <Route path="/ControlloUtenti" element={(grado === "Admin" && SizeG === false ? <ControlloUtenti /> : null)} />
            {lista.map((u, i) =>
              <Route path={"/Automatico/" + u.key} element={(grado === "Admin" || grado === "Basic" ? <Automatico key={i} mac={u.key} /> : null)} />
            )}
            {lista.map((u, i) =>
              <Route path={"/Manuale/" + u.key} element={(grado === "Admin" || grado === "Basic" ? <Manuale key={i} mac={u.key} /> : null)} />
            )}
          </Route>
        </Routes >
      </HashRouter>
    </div>
  );
}
export default App;
