import { useCallback, useState } from "react";

export function NuovoAccount() {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [istrue, setIstrue] = useState(false);
    const [am, setAm] = useState(false);

    const aggiunta = useCallback(async () => {
        if(username!== "" && password !=="")
        {
            let res = await fetch("/Login/AggiuntaAccount", { body: JSON.stringify({ username, password, am }), method: "POST", headers: { 'Content-type': 'application/json; charl set=UTF-8' } });
            if (res.status === 200) {
                setIstrue(true);
            }
        }
    }, [username, password, am]);


    const invio = (event: { key: any; }) => {
        if (event.key === "Enter") {
            aggiunta();
        }
    }
    const p = () => { setAm(!am) };

    

    

    return (
            <div className="CP">
                <div className="titolo fw-bolder" >Aggiungere un Account</div>
                <div className=" form-floating  md-3 UserNametex has-validation ">
                    <input type="text" value={username} className={"form-control is-"+ (istrue === true ? "valid" : "")} placeholder=" " onChange={(a) => { setUsername(a.target.value); }} />
                    <label  form="inputEmail3">Nome Utente</label>
                
                </div>
                <div className=" form-floating is-valid md-3 Passwordtex">
                    <input type="Password" value={password} className={"form-control is-" + (istrue === true ? "valid" : "")} placeholder=" " id="Password3" onChange={(a) => { setPassword(a.target.value); }} onKeyDown={invio} />
                    <label form="inputPassword3">Password</label>
                </div>
                <div className="col-12">
                    <div className="form-check">
                        <input className="form-check-input " type="checkbox" checked={am} onChange={p} id="invalidCheck" required />
                        <label className="form-check-label aministratore">Aministratore</label>
                    </div>
                </div>
                <button type="button"  className={"BouttonSignin btn btn-primary"} onClick={aggiunta}>Aggiungi l'account</button>
            </div>
    );
}


