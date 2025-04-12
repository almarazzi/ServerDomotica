import { useCallback, useEffect, useState } from "react";

export function CambiaPassword() {
    const [Username, setUsername] = useState("");
    const [PasswordAtt, setPasswordAtt] = useState("");
    const [PasswordNuv, setPasswordNuv] = useState("");
    const [PasswordRib, setPasswordRib] = useState("");
    const [invalid, setInvalid] = useState(false);
    const [confronto, setConfronto] = useState(true);
    const [pp, setPp] = useState(false);
    const [statobottone, setStatobottone] = useState(false);

    useEffect(() => {
        if(invalid===true)
        {
            setTimeout(() =>{
                if(PasswordNuv!==null)
                    if (PasswordRib === PasswordNuv) {
                        setConfronto(true);
                        setPp(true);
                    }
                    if (PasswordRib !== PasswordNuv) {
                        setConfronto(false);
                    } 
            },500);
        }
    }, [PasswordNuv, PasswordRib,invalid]);

    const Cambio = useCallback(async () => {

        if (pp === true) {
            let res = await fetch("/Login/cambiaPassword", { body: JSON.stringify({ Username, PasswordAtt, PasswordNuv }), method: "PUT", headers: { 'Content-type': 'application/json; charl set=UTF-8' } });
            if (res.status === 200) {
                setStatobottone(true);
            } else {
                setStatobottone(false);
            }
        }

    }, [pp, Username, PasswordAtt,PasswordNuv]);

    const invio = (event: { key: any; }) => {
        if (event.key === "Enter") {
            Cambio();
        }
    }

    return (
            <div className="CP">
                <div className="titolo1 fw-bolder"> CambiaPassword </div>
                <div className=" form-floating is-invalid md-3 UserNamet">
                    <input type="text" value={Username} className={"form-control"} placeholder=" " id="inputEmail3" onChange={(a) => { setUsername(a.target.value); }} />
                    <label form="inputEmail3">UserName</label>
                </div>

                <div className=" form-floating is-invalid is-valid md-3 Passwordt">
                    <input type="Password" value={PasswordAtt} className={"form-control"} placeholder=" " id="Password1" onChange={(a) => { setPasswordAtt(a.target.value); }} />
                    <label form="Password">Password attuale</label>
                </div>

                <div className=" form-floating is-invalid is-valid md-3 Passwordt">
                    <input type="Password" value={PasswordNuv} className={"form-control"} placeholder=" " id="Password2" onChange={(a) => { setPasswordNuv(a.target.value); }} />
                    <label form="Password">Password Nuova</label>
                </div>

                <div className=" form-floating is-invalid is-valid md-3 Passwordt">
                    <input type="Password" value={PasswordRib} className={"form-control is-" + (confronto === true ? "" : "invalid")} placeholder=" " id="Password3" onChange={(a) => { setPasswordRib(a.target.value);  setInvalid(true); }} onKeyDown={invio} />
                    <label form="Password">Conferma Password</label>
                </div>
                <button type="button" className={"Bouttont btn btn-" + (statobottone === true ? "success" : "primary")} onClick={Cambio}>Cambio</button>
            </div>

    );
}