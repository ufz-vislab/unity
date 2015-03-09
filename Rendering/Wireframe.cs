/*
wireframe update benjamin kiesewetter 2013
faster
normals
vertex extensions
*/

using UnityEngine;
using System.Collections;

public class wireframe : MonoBehaviour {

	public bool render_mesh = true;
	public bool render_wiresframe = true;
	public float normal_length = 1f;
	public float vertext_extention_length = 1f;
	public float lineWidth = 1;
	public Color lineColor = new Color (0.0f, 1.0f, 1.0f);
	public Color backgroundColor = new Color (0.0f, 0.5f, 0.5f);
	public bool ZWrite = true;
	public bool AWrite = true;
	public bool blend = true;

	public int size = 0;
	public int ignored =0;

	private Vector3[] points_a;
	private Vector3[] points_b;
	private Vector3[] vertices;
	private Vector3[] vertex_extensions;
	private Vector3[] normals_center;
	private Vector3[] normals;
	public Material lineMaterial ;

	void Start () {
		if (lineMaterial == null ) {
			lineMaterial = new Material ("Shader \"Lines/Colored Blended\" {" +
										"SubShader { Pass {" +
										"   BindChannels { Bind \"Color\",color }" +
										"   Blend SrcAlpha OneMinusSrcAlpha" +
										"   ZWrite on Cull Off Fog { Mode Off }" +
										"} } }");
		}

		lineMaterial.hideFlags = HideFlags.HideAndDontSave;
		lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;

		// find vertices
		MeshFilter filter  = gameObject.GetComponent<MeshFilter>();
		vertices = filter.mesh.vertices;
		vertex_extensions = new Vector3[vertices.Length];

		// find wire lines and normals by triangles
		int[] triangles = filter.mesh.triangles;
		ArrayList points_a_List = new ArrayList(); //first points of wireframe lines
		ArrayList points_b_List = new ArrayList(); //second points of wireframe lines
		ArrayList normals_center_List = new ArrayList();
		ArrayList normals_List = new ArrayList();

		for (int i = 0; i+2 < triangles.Length; i+=3)
		{
			//for rEaDaBiLiTy
			Vector3 a = vertices[triangles[i]];
			Vector3 b = vertices[triangles[i + 1]];
			Vector3 c = vertices[triangles[i + 2]];

			/* Make the Lines:
				evry line may border two triangles
				so to not render evry line twice
				compare new lines to existing*/
			bool[] line_exists = new bool[]{false,false,false};
			for (int j=0; j<size; j++){
				if (points_a_List[j].Equals(a)){
					if      (points_b_List[j].Equals(b)){
						line_exists[0]= true;
					}else if(points_b_List[j].Equals(c)){
						line_exists[2]= true;
					}
				}else if (points_a_List[j].Equals(b)){
					if      (points_b_List[j].Equals(a)){
						line_exists[0]= true;
					}else if(points_b_List[j].Equals(c)){
						line_exists[1]= true;
					}
				}else  if (points_a_List[j].Equals(c)){
					if      (points_b_List[j].Equals(a)){
						line_exists[2]= true;
					}else if(points_b_List[j].Equals(b)){
						line_exists[1]= true;
					}
				}
			}
			// only add lines if they dont yet exist
			if(!line_exists[0]){
				points_a_List.Add(a);
				points_b_List.Add(b);
				size++;
			} else {
				ignored++;
			}
			if(!line_exists[1]){
				points_a_List.Add(b);
				points_b_List.Add(c);
				size++;
			} else {
				ignored++;
			}
			if(!line_exists[2]){
				points_a_List.Add(c);
				points_b_List.Add(a);
				size++;
			}
			else {
				ignored++;
			}

			// Make the Normals

			//center of triangle
			normals_center_List.Add((a+b+c)*(1f/3f));
			//normal of triangle
			normals_List.Add(Vector3.Cross(b - a, c - a).normalized);
		}

		//arrays are faster than array lists
		points_a = (Vector3[]) points_a_List.ToArray(typeof(Vector3));
		points_a_List.Clear();//free memory from the arraylist
		points_b = (Vector3[]) points_b_List.ToArray(typeof(Vector3));
		points_b_List.Clear();//free memory from the arraylist

		normals_center = (Vector3[]) normals_center_List.ToArray(typeof(Vector3));
		normals_center_List.Clear();//free memory from the arraylist
		normals = (Vector3[]) normals_List.ToArray(typeof(Vector3));
		normals_List.Clear();//free memory from the arraylist
	}

	private float vertext_extention_length_old = 0;

	void update_vertex_extension_length(){
		/* asuming the length of the vertex extensions to barely change
		 * only calculate this if really nessecairy,
		 * increases memory but should speed up*/
		if(vertext_extention_length_old != vertext_extention_length){
			vertext_extention_length_old = vertext_extention_length;
			for(int i = 0; i<vertices.Length; i++){
				vertex_extensions[i]=vertices[i].normalized*vertext_extention_length;
			}
		}
	}

	private float normal_length_old = 0;

	void update_normal_length(){

		/* asuming the length of the normals to barely change
		 * only calculate this if really nessecairy,
		 * increases memory but should speed up*/
		if(normal_length_old != normal_length){
			normal_length_old = normal_length;
			for(int i = 0; i<normals.Length; i++){
				normals[i]=normals[i].normalized*normal_length;
			}
		}
	}

	// to simulate thickness, draw line as a quad scaled along the camera's vertical axis.
	void DrawQuad(Vector3 p1,Vector3 p2 ){
		float thisWidth = 1.0f/Screen.width * lineWidth * 0.5f;
		Vector3 edge1 = Camera.main.transform.position - (p2+p1)/2.0f;  //vector from line center to camera
		Vector3 edge2 = p2-p1;  //vector from point to point
		Vector3 perpendicular = Vector3.Cross(edge1,edge2).normalized * thisWidth;

		GL.Vertex(p1 - perpendicular);
		GL.Vertex(p1 + perpendicular);
		GL.Vertex(p2 + perpendicular);
		GL.Vertex(p2 - perpendicular);
	}


	Vector3 to_world(Vector3 vec)
	{
		return gameObject.transform.TransformPoint(vec);
	}

	void OnRenderObject () {
		gameObject.GetComponent<Renderer>().enabled=render_mesh;
		if (size >  3){
			lineMaterial.SetPass(0);
			GL.Color(lineColor);

			if (lineWidth == 1) {
				GL.Begin(GL.LINES);
				if(render_wiresframe){
					for(int i = 0; i<size; i++)
					{
						GL.Vertex(to_world(points_a[i]));
						GL.Vertex(to_world(points_b[i]));
					}
				}
				if(normal_length>0){
					update_normal_length();
					for(int i = 0; i<normals.Length; i++){
						Vector3 center = to_world(normals_center[i]);
						GL.Vertex(center);
						GL.Vertex(center+normals[i]);
					}
				}
				if(vertext_extention_length > 0){
					update_vertex_extension_length();
					for(int i = 0; i<vertices.Length; i++){
						Vector3 vertex = to_world(vertices[i]);
						GL.Vertex(vertex);
						GL.Vertex(vertex+vertex_extensions[i]);
					}
				}
			} else {
				GL.Begin(GL.QUADS);
				for(int i = 0; i <size; i++) {
					DrawQuad(to_world(points_a[i]),to_world(points_b[i]));
				}
				if(vertext_extention_length > 0){
					update_vertex_extension_length();
					for(int i = 0; i<vertices.Length; i++){
						Vector3 vertex = to_world(vertices[i]);
						DrawQuad(vertex,vertex+vertex_extensions[i]);
					}
				}
				if(normal_length>0){
					update_normal_length();
					for(int i = 0; i<normals.Length; i++){
						Vector3 center = to_world(normals_center[i]);
						DrawQuad(center,center+normals[i]);
					}
				}
			}
			GL.End();
		}else{
			print("No lines");
		}
	}
}
