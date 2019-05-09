using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelBuffer : MonoBehaviour
{
    public ComputeShader _texFiller;

    public ComputeShader _voxMeshGen;

    private ComputeBuffer _vertexBuffer;

    private RenderTexture _voxelData;
    
    
    public 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
