using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MezzClass0._1.Models;
using Microsoft.Owin.Security.Provider;


namespace MezzClass0._1.Controllers
{
    public class TProblemasController : Controller
    {
        private MezClassEntities db = new MezClassEntities();

        // GET: TProblemas
        public ActionResult Index(Int32? id ,string p,string e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            IQueryable<TProblema> tProblema;
            if (e!=null)
            {
                if (e == "El problema debe tener un parametro o mas")
                    ViewBag.error = "El problema debe tener un parametro o mas";
                else
                ViewBag.error = "Ya existe un Problema titulo "+e +" en este Universo";
            }
            if (p != null)
            {
                tProblema = db.TProblema.Include(t => t.TUniverso).Where(param => param.IdUniverso == id & param.usuario==p);
                ViewBag.id = id;
                ViewBag.bandera = 1;
                return View(tProblema.ToList());
            }
            tProblema = db.TProblema.Include(t => t.TUniverso).Where(param => param.IdUniverso == id);
            ViewBag.id = id;
            return View(tProblema.ToList());
        }

        // GET: TProblemas/Details/5
        public ActionResult Details(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TProblema tProblema = db.TProblema.Find(id);
          
            if (tProblema == null)
            {
                return HttpNotFound();
            }
            DirectoryInfo directory = new DirectoryInfo(@"E:\Files\" + tProblema.IdProblema);

            if (Directory.Exists(@"E:\Files\" + tProblema.IdProblema))
            {
                FileInfo[] rgFiles = directory.GetFiles("*.*");
                List<string> items = rgFiles.Select(file => file.Name).ToList();
                if (items.Count > 0)
                {
                    ViewBag.items = items;
                }
            }
            return View(tProblema);
        }

       

        // GET: TProblemas/Create
        public ActionResult Create(Int32? id, Int32? bandera)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.IdUniverso = id;
            var u = db.TUniverso.Where(param => param.IdUniverso == id).ToList();
           
            ViewBag.nomb = u[0].NombUniverso;
        
            var parametros = db.TParametro.Where(param => param.IdUniverso == id).ToList();
            if (bandera != null)
            {
                ViewBag.bandera = 1;
            }
            ViewBag.parametros = parametros;
            return View();
        }

        // POST: TProblemas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdProblema,titulo,Enunciado,Solucion,IdUniverso,ejemplo,ComplejidadId,usuario")] TProblema tProblema, string json, HttpPostedFileBase file,
            HttpPostedFileBase imagen, HttpPostedFileBase documento)
        {
 
            List<TParametro> parametros;
            List<TUniverso> u;
            bool band = false;
            if (ModelState.IsValid)
            {
                try
                {
                    var x1 = db.TProblema.Where(
                                      param => param.titulo == tProblema.titulo && param.IdUniverso == tProblema.IdUniverso).ToList();
                    if (x1.Count > 0)
                    {
                        ViewBag.IdUniverso = new SelectList(db.TUniverso, "IdUniverso", "NombUniverso", tProblema.IdUniverso);
                        ViewBag.error = "Ya existe un problema con el mismo titulo en este Universo";
                        parametros = db.TParametro.Where(param => param.IdUniverso == tProblema.IdUniverso).ToList();
                        ViewBag.parametros = parametros;
                        u = db.TUniverso.Where(param => param.IdUniverso == tProblema.IdUniverso).ToList();
                        ViewBag.nomb = u[0].NombUniverso;
                        ViewBag.IdUniverso = tProblema.IdUniverso;
                        return View(tProblema);
                    }

                    if (tProblema.ejemplo == false)
                        tProblema.Solucion = null;
                   
                    List<TProblemaParametro> t = ParseParametros(json);
                    if (t.Count == 0)
                    {
                        ViewBag.IdUniverso = new SelectList(db.TUniverso, "IdUniverso", "NombUniverso", tProblema.IdUniverso);
                        ViewBag.error = "Agregue al menos un parametro para el problema";
                        parametros = db.TParametro.Where(param => param.IdUniverso == tProblema.IdUniverso).ToList();
                        ViewBag.parametros = parametros;
                        u = db.TUniverso.Where(param => param.IdUniverso == tProblema.IdUniverso).ToList();
                        ViewBag.nomb = u[0].NombUniverso;
                        ViewBag.IdUniverso = tProblema.IdUniverso;
                        return View(tProblema);
                    }
                    if (t.Count > 0)
                        tProblema.TProblemaParametro = t;

                    SetComplexity(tProblema,tProblema.TProblemaParametro);
                    db.TProblema.Add(tProblema);
                    int x = db.SaveChanges();
                    band = true;
                }
                catch (Exception E)
                {
                    ViewBag.IdUniverso = new SelectList(db.TUniverso, "IdUniverso", "NombUniverso", tProblema.IdUniverso);
                  
                    ViewBag.error = "Ha ocurrido un error cuando se creaba el problema. Revise los campos";
                    parametros = db.TParametro.Where(param => param.IdUniverso == tProblema.IdUniverso).ToList();
                    ViewBag.parametros = parametros;
                    u = db.TUniverso.Where(param => param.IdUniverso == tProblema.IdUniverso).ToList();
                    ViewBag.nomb = u[0].NombUniverso;
                    ViewBag.IdUniverso = tProblema.IdUniverso;
                    return View(tProblema);
                }

                if (band == true)
                {
                    DirectorySecurity s = new DirectorySecurity("E:", AccessControlSections.Owner);
                    DirectoryInfo directory = new DirectoryInfo("E:");
                    directory.SetAccessControl(s);
                    if (Directory.Exists(@"E\Files") == false)
                    {
                        Directory.CreateDirectory(directory.Root.Name + @"\Files");
                    }
                    Directory.CreateDirectory(directory.Root.Name + @"Files\" + tProblema.IdProblema);

                    if (file != null)
                    {
                        string ext = SaberExt(file.FileName);
                        if (CompararCadenas(ext, "avi") || CompararCadenas(ext, "mp4"))
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var path =
                                Path.Combine(
                                    directory.Root.Name + @"Files\" + tProblema.IdProblema,
                                    fileName);
                            file.SaveAs(path);
                        }
                    }
                    if (imagen != null)
                    {
                        string extimagen = SaberExt(imagen.FileName);
                        if (CompararCadenas(extimagen, "jpg") || CompararCadenas(extimagen, "png") || CompararCadenas(extimagen, "jpeg"))
                        {
                            var fileName = Path.GetFileName(imagen.FileName);
                            var path = Path.Combine(directory.Root.Name + @"Files\" + tProblema.IdProblema, fileName);
                            imagen.SaveAs(path);
                        }
                    }

                    if (documento != null)
                    {
                        string extdoc = SaberExt(documento.FileName);
                        if (extdoc.Contains("xls") || extdoc.Contains("doc") || CompararCadenas(extdoc, "pdf"))
                        {
                            var fileName = Path.GetFileName(documento.FileName);
                            var path = Path.Combine(directory.Root.Name + @"Files\" + tProblema.IdProblema, fileName);
                            documento.SaveAs(path);
                        }
                    }
              
                }
               
                return RedirectToAction("Index", new {id = tProblema.IdUniverso,p=tProblema.usuario});


            }

            ViewBag.IdUniverso = new SelectList(db.TUniverso, "IdUniverso", "NombUniverso", tProblema.IdUniverso);
          
            parametros = db.TParametro.Where(param => param.IdUniverso == tProblema.IdUniverso).ToList();
            ViewBag.parametros = parametros;
            u = db.TUniverso.Where(param => param.IdUniverso == tProblema.IdUniverso).ToList();
            ViewBag.nomb = u[0].NombUniverso;
            return View(tProblema);
        }

        // GET: TProblemas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TProblema tProblema = db.TProblema.Find(id);
            if (tProblema == null)
            {
                return HttpNotFound();
            }
            DirectoryInfo directory = new DirectoryInfo(@"E:\Files\" + tProblema.IdProblema);

            if (Directory.Exists(@"E:\Files\" + tProblema.IdProblema))
            {
                FileInfo[] rgFiles = directory.GetFiles("*.*");
                List<string> items = new List<string>();
                foreach (var file in rgFiles)
                {
                    items.Add(file.Name);
                }
                if (items.Count > 0)
                {
                    ViewBag.items = items;
                }
            }
          

           
          

            var pp = db.TProblemaParametro.Where(p => p.IdProblema == id).ToList();
            var parametros = db.TParametro.Where(param => param.IdUniverso == tProblema.IdUniverso).ToList();
            foreach (TProblemaParametro t in pp)
            {
                for (int j = 0; j < parametros.Count; j++)
                {
                    if(parametros[j].IdParametro==t.IdParametro)
                    {parametros.RemoveAt(j);
                        j--;
                    }
                }
            }
            ViewBag.parametros = parametros;
            ViewBag.IdUniverso = new SelectList(db.TUniverso, "IdUniverso", "NombUniverso", tProblema.IdUniverso);
            

            
            
            return View(tProblema);
        }

        // POST: TProblemas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdProblema,titulo,Enunciado,Solucion,IdUniverso,ejemplo,ComplejidadId,usuario")] TProblema tProblema, string json, HttpPostedFileBase file,
            HttpPostedFileBase imagen, HttpPostedFileBase documento)
        {
            if (ModelState.IsValid)
            {
                var probs =
                    db.TProblema.Where(p => p.IdUniverso == tProblema.IdUniverso && p.IdProblema != tProblema.IdProblema &&p.titulo==tProblema.titulo)
                        .ToList();
                if (probs.Count > 0)
                {
                    
                    return RedirectToAction("Index", new { id = tProblema.IdUniverso, p = tProblema.usuario,e=tProblema.titulo});
                }
                db.Entry(tProblema).State = EntityState.Modified;
                db.TProblemaParametro.RemoveRange(db.TProblemaParametro.Where(p => p.IdProblema == tProblema.IdProblema));
                List<TProblemaParametro> t = ParseParametros(json);

                if (t.Count == 0)
                {
                    return RedirectToAction("Index", new { id = tProblema.IdUniverso, p = tProblema.usuario, e="El problema debe tener un parametro o mas" });
                }
                if (t.Count > 0)
                {
                    tProblema.TProblemaParametro = t;
                    SetComplexity(tProblema, tProblema.TProblemaParametro);
                }
                
                db.SaveChanges();

                DirectorySecurity s = new DirectorySecurity("E:", AccessControlSections.Owner);
                DirectoryInfo directory1 = new DirectoryInfo("E:");
                directory1.SetAccessControl(s);
                if (Directory.Exists(@"E\Files") == false)
                {
                    Directory.CreateDirectory(directory1.Root.Name + @"\Files");
                }
                Directory.CreateDirectory(directory1.Root.Name + @"Files\" + tProblema.IdProblema);
                    if (file != null)
                    {
                        string ext = SaberExt(file.FileName);
                        if (CompararCadenas(ext,"avi") || CompararCadenas(ext,"mp4"))
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var path =
                                Path.Combine(
                                    directory1.Root.Name + @"Files\" + tProblema.IdProblema,
                                    fileName);
                            file.SaveAs(path);
                        }
                    }
                    if (imagen != null)
                    {
                        string extimagen = SaberExt(imagen.FileName);
                        if (CompararCadenas(extimagen,"jpg") || CompararCadenas(extimagen,"png") || CompararCadenas(extimagen,"jpeg"))
                        {
                            var fileName = Path.GetFileName(imagen.FileName);
                            var path = Path.Combine(directory1.Root.Name + @"Files\" + tProblema.IdProblema, fileName);
                            imagen.SaveAs(path);
                        }
                    }

                    if (documento != null)
                    {
                        string extdoc = SaberExt(documento.FileName);
                        if (extdoc.Contains("xls") || extdoc.Contains("doc") || CompararCadenas(extdoc, "pdf"))
                        {
                            var fileName = Path.GetFileName(documento.FileName);
                            var path = Path.Combine(directory1.Root.Name + @"Files\" + tProblema.IdProblema, fileName);
                            documento.SaveAs(path);
                        }
                    }
                

                return RedirectToAction("Index",new{id=tProblema.IdUniverso,p=tProblema.usuario});
            }

          
           
            ViewBag.IdUniverso = new SelectList(db.TUniverso, "IdUniverso", "NombUniverso", tProblema.IdUniverso);
            return View(tProblema);
        }

        // GET: TProblemas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TProblema tProblema = db.TProblema.Find(id);
           
            if (tProblema == null)
            {
                return HttpNotFound();
            }
            DirectoryInfo directory = new DirectoryInfo(@"E:\Files\" + tProblema.IdProblema);
            if (Directory.Exists(@"E:\Files\" + tProblema.IdProblema))
            {
                FileInfo[] rgFiles = directory.GetFiles("*.*");
                List<string> items = new List<string>();
                foreach (var file in rgFiles)
                {
                    items.Add(file.Name);
                }
                if (items.Count > 0)
                {
                    ViewBag.items = items;
                }
            }
            return View(tProblema);
        }

        // POST: TProblemas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TProblema tProblema = db.TProblema.Find(id);
            db.TProblemaParametro.RemoveRange(db.TProblemaParametro.Where(p => p.IdProblema == tProblema.IdProblema));
            db.TProblema.Remove(tProblema);
            db.SaveChanges();
            DirectoryInfo directory = new DirectoryInfo(@"E:\Files\" + tProblema.IdProblema);
            if (Directory.Exists(@"E:\Files\" + tProblema.IdProblema))
            {
                FileInfo[] dirs = directory.GetFiles("*.*");
                foreach (var x in dirs)
                {
                    System.IO.File.Delete(x.FullName);
                }
                directory.Delete();
             
            }

            return RedirectToAction("Index",new {id=tProblema.IdUniverso,p=tProblema.usuario});
        }


        //Parsers
        public List<TProblemaParametro> ParseParametros(string json)
        {
           
            List<TProblemaParametro> final = new List<TProblemaParametro>();
            char[] separador = {':', ']'};
            List<int> ids = new List<int>();
            List<double> valores = new List<double>();
                
            string[] res = json.Split(separador, StringSplitOptions.RemoveEmptyEntries);
            List<string> re1 = new List<string>();
            for (int i = 1; i < res.Length; i+=2)
            {
                re1.Add(res[i]);
            }
            if (re1.Count > 0)
            {
                foreach (var VARIABLE in re1)
                {
                    char[] separador1 = {','};
                    string[] x = VARIABLE.Split(separador1);
                    string num = "";
                    string value = "";
                    for (int i = 0; i < x[0].Count(); i++)
                    {
                       
                        if (x[0][i] != '/' && x[0][i] != '"' && x[0][i] != '[' && x[0][i] != ']')
                        {
                            num += x[0][i];
                        }
                      
                    }
                    for (int i = 0; i < x[3].Count(); i++)
                    {

                        if (x[3][i] != '/' && x[3][i] != '"' && x[3][i] != '[' && x[3][i] != ']')
                        {
                            value += x[3][i];
                        }

                    }
                   
                    ids.Add(int.Parse(num));
                    valores.Add(double.Parse(value.Replace(".",",")));
                }
            }
            if (ids.Count > 0)
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    TProblemaParametro t = new TProblemaParametro();
                    t.IdParametro = ids[i];
                    t.Valor = valores[i];
                    final.Add(t);
                }
              
            }
            return final;
        }



        private string SaberExt(string name)
        {
            string ext="";
            for (int i = name.Count()-1; i>=0; i--)
            {
                if(name[i]=='.')
                    break;
                ext += name[i];
            }
            string fin = "";
            for (int i=ext.Count()-1 ; i >=0; i--)
            {
                fin += ext[i];
            }
            return fin;
        }


        public FileResult Download(string filename,string ruta)
        {
            return File(ruta + filename, System.Net.Mime.MediaTypeNames.Application.Octet);
        }

        public int ElimDir(Int32 idp,string nombre)
        {
            DirectoryInfo directory = new DirectoryInfo(@"E:\Files\" + idp);
            if (Directory.Exists(@"E:\Files\" + idp))
            {
                FileInfo[] dirs = directory.GetFiles("*.*");
                foreach (var x in dirs)
                {
                    if(x.Name==nombre)
                    { System.IO.File.Delete(x.FullName);
                        return 1;
                    }
                }
              

            }
            return -1;
        }

        public bool CompararCadenas(string x, string y)// Comparar sin importar mayusculas y minusculas.
        {
            if (String.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0)
                return true;
            return false;
        }

        //Complejidad
        private void SetComplexity(TProblema t, ICollection<TProblemaParametro> p)
        {
            
            p = new List<TProblemaParametro>(p.OrderBy(a=>a.Valor));

            if (p.Count == 1)
            {
                t.ComplejidadId = 1;
            }
            else
            {
                List<double> valores = new List<double>();
                foreach (var val in p)
                {
                    if (val.Valor != null) valores.Add((double)val.Valor);
                }
                double inc = valores[0];
                double final = valores[valores.Count - 1];
                List<int> resultados = new List<int>();

                int cont = 1;
                TUniverso u = db.TUniverso.Find(t.IdUniverso);
                for (int i = 0; i < u.CantNiveles; i++)
                {
                    object[] intervalo = Intervals(cont, final, inc);//obtener el nivel donde esta ubicado este valor.
                    foreach (var par in p)
                    {
                        if (Ubicar(intervalo, par))//saber si en que posicion del intervalo esta el parametro
                        {
                            resultados.Add(cont);// Agregarlo a la lista de resultados
                        }
                    }
                    cont++;
                }
                resultados = new List<int>(resultados.OrderByDescending(i => i));
                List<object[]> fin = new List<object[]>();
                for (int i = 0; i < resultados.Count; i++)
                {
                    List<int> repeticiones = new List<int>();
                    int n = resultados[0];
                    repeticiones.Add(n);
                    for (int j = 1; j < resultados.Count; j++)
                    {
                        if (n == resultados[j])
                        {
                            repeticiones.Add(n);
                            resultados.RemoveAt(j);
                            j--;
                        }
                    }
                    resultados.RemoveAt(0);
                    i--;
                    object[] o = new object[2];
                    o[0] = n;
                    o[1] = repeticiones.Count;
                    fin.Add(o);

                }//Obtener repeticiones del parametro en un intervalo
                fin = new List<object[]>(fin.OrderByDescending(k => k[1]));
                if (Iguales(fin))//Si las cantidades son iguales
                {
                    //Teoria de Desicion Laplace
                    t.ComplejidadId = int.Parse(fin[fin.Count-1][0].ToString());

                }
                else
                 {
                 if (fin.Count == 2)
                  {
                    t.ComplejidadId = int.Parse(fin[1][0].ToString());
                  }
                   else
                   {
                       t.ComplejidadId = int.Parse(fin[0][0].ToString());
                   }
                 }
            }
        }

        public bool Iguales(List<object[]> O)//Sabes si los resultados son iguales.
        {
            for (int i = 0; i < O.Count; i++)
            {
                var a = O[i][1];
                
                for (int j = 0; j < O.Count; j++)
                {
                    var b = O[j][1];
                    if (int.Parse(a.ToString()) > int.Parse(b.ToString()) || int.Parse(a.ToString()) < int.Parse(b.ToString()))
                    {
                        return false;
                    }
                }
            }
             return true;
         }
       
        

        private bool Ubicar(object[] intervalo,TProblemaParametro p)
        {
            if (p.Valor >= double.Parse(intervalo[0].ToString()) && p.Valor<= double.Parse(intervalo[1].ToString()))
            {
                return true;
            }
            return false;
        }

        private object[] Intervals(int num,double max,double min)
        {
            object[] fin = new object[2];
            if (num == 1)
            {
              
                fin[0] = min;
                fin[1] = (max + 2 * min)*1/3;
                return fin;
            }
            if (num == 2)
            {
               
                fin[0] = (max + 2 * min) * 1/3;
                fin[1] = (2 * max + min) * 1/3;
                return fin;
            }

            fin[0] = (2 * max + min) * 1/3;
            fin[1] = max;
            return fin;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
