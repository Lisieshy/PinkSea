using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinkSea.Database.Models;

/// <summary>
/// A database oekaki model.
/// </summary>
public class OekakiModel
{
    /// <summary>
    /// The TID of the model.
    /// </summary>
    [Key]
    public required string Tid { get; set; }
    
    /// <summary>
    /// The ID of the oekaki post.
    /// </summary>
    [ForeignKey(nameof(Author))]
    public required string AuthorDid { get; set; }
    
    /// <summary>
    /// When was the oekaki indexed at by the AppView?
    /// </summary>
    public required DateTimeOffset IndexedAt { get; set; }
    
    /// <summary>
    /// The author.
    /// </summary>
    public required UserModel Author { get; set; }

    /// <summary>
    /// The oekaki record cid.
    /// </summary>
    public required string RecordCid { get; set; }
    
    /// <summary>
    /// The image blob cid.
    /// </summary>
    public required string BlobCid { get; set; }
    
    /// <summary>
    /// The alt text of the image.
    /// </summary>
    public string? AltText { get; set; }
    
    /// <summary>
    /// The ID of the parent post.
    /// </summary>
    [ForeignKey(nameof(Parent))]
    public string? ParentId { get; set; }
    
    /// <summary>
    /// The ID of the parent.
    /// </summary>
    public OekakiModel? Parent { get; set; }
}