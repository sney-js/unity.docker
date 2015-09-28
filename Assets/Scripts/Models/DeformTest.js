#pragma strict
// This script is placed in public domain. The author takes no responsibility for any possible harm.
 
var scale = 1.0;
var speed = 1.0;
var recalculateNormals = false;
 /*
private var baseVertices : Vector3[];
private var noise : TreeEditor.Perlin;
 
function Start ()
{
    noise = new TreeEditor.Perlin ();
}
 
function Update () {
    var mesh : Mesh = GetComponent(MeshFilter).mesh;
   
    if (baseVertices == null)
        baseVertices = mesh.vertices;
       
    var vertices = new Vector3[baseVertices.Length];
   
    var timex = Time.time * speed + 0.1365143;
    var timey = Time.time * speed + 1.21688;
    var timez = Time.time * speed + 2.5564;
    for (var i=0;i<vertices.Length;i++)
    {
        var vertex = baseVertices[i];
               
        vertex.x += noise.Noise(timex + vertex.x, timex + vertex.y, timex + vertex.z) * scale;
        vertex.y += noise.Noise(timey + vertex.x, timey + vertex.y, timey + vertex.z) * scale;
        vertex.z += noise.Noise(timez + vertex.x, timez + vertex.y, timez + vertex.z) * scale;
       
        vertices[i] = vertex;
    }
   
    mesh.vertices = vertices;
   
    if (recalculateNormals)
        mesh.RecalculateNormals();
    mesh.RecalculateBounds();
}
*/

/*
	Vector3[] originalMesh;
//	public float dentFactor;
	public LayerMask collisionMask;
	public MeshFilter meshFilter;
	public float DeformScale = 4.0f;

			originalMesh = meshFilter.mesh.vertices
//----------------------------------------------mesh change------------------------------------
		if (meshFilter != null) {
			Vector3[] meshCoordinates = originalMesh;
			// Loop through collision points
			foreach (ContactPoint2D point in other.contacts) {
				// Index with the closest distance to point.
				Vector3 impactPoint = new Vector3 (point.point.x, point.point.y, -1f);
				int lastIndex = 0;
				Queue<int> set = new Queue<int> ();
				// Loop through mesh coordinates
				for (int i = 0; i < meshCoordinates.Length; i++) {
					// Check to see if there is a closer index
					if (Vector3.Distance (impactPoint, meshCoordinates [i])
						< Vector3.Distance (impactPoint, meshCoordinates [lastIndex])) {
						// Set the new index
						lastIndex = i;
						set.Enqueue (lastIndex);
						if (set.Count >= 5)
							set.Dequeue ();
					}
				}
//				TreeEditor.Perlin noise = new TreeEditor.Perlin();
				foreach (int vert in set) {
//					meshCoordinates [vert] += new Vector3 (Random.Range (-DeformScale, DeformScale),
//					                                            Random.Range (-DeformScale, DeformScale),
//					                                            Random.Range (-DeformScale, DeformScale));
//					
					meshCoordinates [vert] -= meshCoordinates [lastIndex].normalized * DeformScale;
				}
				// Move the vertex
				
			}
			meshFilter.mesh.vertices = meshCoordinates;
		}
		*/