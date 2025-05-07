import { useCallback, useEffect, useRef, useState } from "react";
import "@babylonjs/loaders/stl";
import "@babylonjs/core/Culling/ray";
import { Engine } from "@babylonjs/core/Engines/engine";
import { Scene } from "@babylonjs/core/scene";
import { ArcRotateCamera } from "@babylonjs/core/Cameras/arcRotateCamera";
import { Vector3, Color3 } from "@babylonjs/core/Maths/math";
import { Mesh } from "@babylonjs/core/Meshes/mesh";
import { HemisphericLight } from "@babylonjs/core/Lights/hemisphericLight";
import { LoadAssetContainerAsync } from "@babylonjs/core/Loading";
import { StandardMaterial } from "@babylonjs/core/Materials/standardMaterial";
import { CreateCylinder } from "@babylonjs/core/Meshes/Builders/cylinderBuilder";

//import {Inspector} from  '@babylonjs/inspector';  //inspector indeciso se lasciarlo o no 
import Automatico from "./Automatico";
import { Manuale } from "./Manuale";
interface Lista {
  readonly nomeEspClient: string;
  readonly ipEsp: string;
  readonly abilitazione: boolean;
}
interface key {
  key: string;
  value: Lista;
}

interface Tutto {
  state: boolean,
  macricever: string
}

export function Babylon(props: { mac: Array<key> }) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const containerRef = useRef<HTMLDivElement>(null);
  const scenaRef = useRef<Scene>();
  const [visibilita, setVisibilita] = useState(false);
  const [key, setkey] = useState(0);
  const [oggetto, setoggetto] = useState<Mesh[]>([] as Mesh[]);
  const [mac, setMac] = useState("");
  const [RelayActive, setRelayActive] = useState([] as Tutto[]);
  const [A, setA] = useState(false);
  const [Auto, setAtuo] = useState(false);
  const [M, setM] = useState(false);

  useEffect(() => {
    const contenitore = containerRef.current;
    const canvas = canvasRef.current;
    const engine = new Engine(canvas, true);
    const scene = new Scene(engine);
    scenaRef.current = scene;
    const camera = new ArcRotateCamera("camera", 4.79, 1.27, 29.9, new Vector3(11.69, 1.82, -1.79), scene);
    camera.attachControl(canvas, true);
    new HemisphericLight("light", new Vector3(2, 1, 0), scene);
    const STL = async () => {
      const casa = await LoadAssetContainerAsync("/casa.stl", scene);
      var g = new StandardMaterial("g", scene);
      g.diffuseColor = new Color3(0.61, 0.61, 0.61);
      casa.meshes.map((u, _) => {
        u.material = g;
      })
      casa.addToScene();
    }
    STL();
    engine.runRenderLoop(() => {
      scene.render();
    });
    /*Inspector.Show(scene, {  //inspector indeciso se lasciarlo o no 
      embedMode:true,
      overlay:true,
      showInspector:true,
      showExplorer:true
    });*/

    const dimenzioni = () => {
      if (canvas && contenitore) {
        canvas.width = contenitore.clientWidth;
        canvas.height = contenitore.clientHeight;
        engine.resize();
      }
    };
    const oggettoDimenzioni = new ResizeObserver(dimenzioni);
    if (contenitore)
      oggettoDimenzioni.observe(contenitore);

    dimenzioni();
    return () => {
      oggettoDimenzioni.disconnect();
      engine.dispose();
    };
  }, []);

  useEffect(() => {
    if (oggetto.length !== props.mac.length) {
      setoggetto(props.mac.map((u, i) => {
        return irrigazione({ name: u.key, x: 17.5, z: i });
      }));
    }
    else
      setoggetto(oggetto);
  }, [props.mac]);

  useEffect(() => {
    if (scenaRef.current)
      scenaRef.current.onPointerDown = function (_, pickResult) {
        if (pickResult.hit && pickResult.pickedMesh) {
          const ogg = oggetto.find((u) => u.name === pickResult.pickedMesh?.name);
          if (ogg) {
            setVisibilita(true);
            setMac(ogg.name);
          }
        }
      };
  }, [oggetto]);

  useEffect(() => {
    RelayActive.map((h, _) => {
      const ogg = oggetto.find((u) => u.name === h.macricever);
      if (h.state === true && ogg?.position.y === 2.1) {
        animazioneSu({ oggetto: ogg, tinziale: performance.now() });
      }
      else if (h.state === false && ogg?.position.y === 2.9) {
        animazioneGiu({ oggetto: ogg, tinziale: performance.now() });
      }
    });
  }, [RelayActive]);


  useEffect(() => {
    let active = true;
    const fetchApi = async () => {
      let data = await fetch("/api/RelaySwitch/GetState", { method: 'GET', headers: { 'Content-type': 'application/json; charl set=UTF-8' } });
      var res = await data.json() as Tutto[];
      if (active) {
        setRelayActive(res);
        setTimeout(() => {
          fetchApi();
        }, 500);
      }
    };
    fetchApi();
    return () => {
      active = false;
    };
  }, []);



  function irrigazione(props: { name: string, x: number, z: number }) {
    var irrigazione = CreateCylinder(props.name, { height: 1, diameter: 0.2 });
    var r = new StandardMaterial("");
    r.diffuseColor = new Color3(0.45, 0.56, 0.53);
    irrigazione.material = r;
    irrigazione.position.x = props.x;
    irrigazione.position.y = 2.1;
    irrigazione.position.z = props.z;
    return irrigazione;
  };

  function animazioneSu(props: { oggetto: Mesh; tinziale: number }) {
    props.oggetto.onBeforeRenderObservable.add(() => {
      const t = (performance.now() - props.tinziale) / 1000;
      if (t < 1) {
        props.oggetto.position.y = 2.1 + Math.sin(t * (Math.PI / 2)) * (2.9 - 2.1);
      }
      else {
        props.oggetto.position.y = 2.9;
      }
    });
  }

  function animazioneGiu(props: { oggetto: Mesh; tinziale: number }) {
    props.oggetto.onBeforeRenderObservable.add(() => {
      const t = (performance.now() - props.tinziale) / 1000;
      if (t < 1) {
        props.oggetto.position.y = 2.9 - Math.sin(t * (Math.PI / 2)) * (2.9 - 2.1);
      }
      else {
        props.oggetto.position.y = 2.1;
      }
    });
  }
  useEffect(() => {
    setkey(key + 1);
  }, [mac])


  const X = useCallback(() => {
    setVisibilita(false);
  }, [visibilita]);

  //abi
  const p1 = useCallback(async () => {
    const api = async () => {
      await fetch("/apiEsp/abilitazione", { body: JSON.stringify({ abilitazione: !A, mac: mac }), method: "PUT", headers: { 'Content-type': 'application/json; charl set=UTF-8' } });
    }
    if (mac !== "") {
      api();
    }
  }, [A, mac]);

  useEffect(() => {

    props.mac.map((u, _) => {
      if (u.key === mac)
        setA(u.value.abilitazione);

    })
  }, [props.mac, mac]);

  //manu
  const m1 = useCallback(async () => {
    if (mac !== "") {
      const inv = { stateProgrammManu: !M, macricever: mac };
      await fetch("/api/RelaySwitch/stateProgrammManu", { method: "PUT", body: JSON.stringify(inv), headers: { 'Content-type': 'application/json; charl set=UTF-8' } });
    }
  }, [M, mac]);
  useEffect(() => {
    let isactive = true;
    const fetchData = async () => {
      let data = await fetch("/api/RelaySwitch/GetProgrammManu", { method: 'GET', headers: { 'Content-type': 'application/json; charl set=UTF-8' } });
      var res = await data.json() as Tutto[];
      if (isactive) {
        {
          res.map((u, _) => {
            if (u.macricever === mac) {
              setM(u.state)
            }
          })
        }
        setTimeout(() => {
          fetchData();
        }, 500);
      }
    };
    fetchData();
    return () => {
      isactive = false;
    };
  }, [mac]);
  //auto

  const a1 = useCallback(async () => {
    if (mac !== "") {
      const inv = { stateProgrammAuto: !Auto, macricever: mac };
      await fetch("/api/RelaySwitch/stateProgrammAuto", { method: "PUT", body: JSON.stringify(inv), headers: { 'Content-type': 'application/json; charl set=UTF-8' } })
    }
  }, [Auto, mac]);

  useEffect(() => {
    let isactive = true;
    const fetchData1 = async () => {
      let data = await fetch("/api/RelaySwitch/GetProgrammAuto", { method: 'GET', headers: { 'Content-type': 'application/json; charl set=UTF-8' } });
      let res = await data.json() as Tutto[];
      if (isactive) {
        {
          res.map((u, _) => {
            if (u.macricever === mac) {
              setAtuo(u.state)
            }
          })
        }
        setTimeout(() => {
          fetchData1();
        }, 500);
      }
    };
    fetchData1();
    return () => {
      isactive = false;
    };
  }, [mac]);

  return (
    <div ref={containerRef} className="contenitore">

      {visibilita && (
        <div className="finestra">
          <button className="btn btn-primary" onClick={X}>X</button>
          <div>
            <h4 className="titolo">{mac}</h4>
            <div>
              <input className="form-check-input" type="checkbox" checked={A} onChange={p1} id="invalidCheck1" required />
              <label form="abiliatazione"> Abilitazione Esp </label>
            </div>
            <hr />
            <div>
              <input className="form-check-input" type="checkbox" checked={M} onChange={m1} id="invalidCheck1" required />
              <label form="abiliatazione"> Abilitazione Manuale </label>
            </div>
            <Automatico key={key} mac={mac} />
            <hr />
            <div>
              <input className="form-check-input" type="checkbox" checked={Auto} onChange={a1} id="invalidCheck1" required />
              <label form="abiliatazione"> Abilitazione Automatico </label>
            </div>
          </div>
          <div className="manu">
            <Manuale key={key + 1} mac={mac} />
          </div>
        </div>
      )}

      <canvas ref={canvasRef} />
    </div>
  );
}




