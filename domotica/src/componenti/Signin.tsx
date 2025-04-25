import  { useCallback, useState } from "react";

export function Signin(props: { setToken: (t: boolean) => void}) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [invalid, setInvalid] = useState(false);
  const [visibilita, setvisibilita] = useState(false);
  
  const signIn = useCallback(async () => {
    let res = await fetch("/Login/cookie", { body: JSON.stringify({ username, password}), method: "POST", headers: {'Content-type': 'application/json; charl set=UTF-8'}});
    if (res.status === 200)
    {
      props.setToken(true);
     // setLoading(false);
      setvisibilita(false);
    }else
    {
      props.setToken(false);
      setInvalid(true);
      setvisibilita(false);
    }
  }, [username, password,props]);

  const invio = (event: { key: any; }) => {
    if (event.key === "Enter") {
      signIn();
      setvisibilita(true);
    }
  }

  return (
      <div className="aa">
        <div className="titolo fw-bolder" >centralina di irrigazione </div>
        <div className=" form-floating  md-3 UserNametex is-invalid " >
          <input type="text" value={username} className={" form-control is-" + (invalid === true ? "invalid" : "")}  placeholder=" " id="inputNomeUtente" onChange={(a) => { setUsername(a.target.value); }} />
          <label form="inputNomeUtente">UserName</label>
        </div>

        <div className="form-floating is-invalid md-3 Passwordtex">
          <input type="password" value={password} className={"form-control is-" + (invalid === true ? "invalid" : "")} placeholder=" " id="inputPassword" onChange={(a) => { setPassword(a.target.value) }} onKeyDown={invio} />
          <label form="inputPassword">Password</label>
        </div>

      <button type="button" className="btn btn-success Pb_login" onClick={()=> {signIn(), setvisibilita(true)}}>
        <span role="status"> Login </span>
        {visibilita && <span className="spinner-border spinner-border-sm" aria-hidden="true"></span>}
      </button>
        
        
      </div> 
  );

}

